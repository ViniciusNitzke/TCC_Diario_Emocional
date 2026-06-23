import React, { useEffect, useMemo, useState } from 'react';
import { AppHeader } from './components/AppHeader.jsx';
import { AuthPage } from './components/AuthPage.jsx';
import { DashboardMetrics } from './components/DashboardMetrics.jsx';
import { EntryForm } from './components/EntryForm.jsx';
import { ReportView } from './components/ReportView.jsx';
import { SettingsView } from './components/SettingsView.jsx';
import { SupportPanel } from './components/SupportPanel.jsx';
import { TimelineView } from './components/TimelineView.jsx';
import { emptyEntryForm, tokenKey } from './constants/emotionalCatalog.js';
import { createApiClient } from './services/apiClient.js';

export default function App() {
  const [token, setToken] = useState(() => localStorage.getItem(tokenKey));
  const [user, setUser] = useState(null);
  const [catalog, setCatalog] = useState([]);
  const [entries, setEntries] = useState([]);
  const [dashboard, setDashboard] = useState(null);
  const [report, setReport] = useState(null);
  const [crisis, setCrisis] = useState(null);
  const [activeTab, setActiveTab] = useState('diario');
  const [entryForm, setEntryForm] = useState(emptyEntryForm);
  const [authMode, setAuthMode] = useState('login');
  const [authForm, setAuthForm] = useState({ nome: 'Usuário Demo', email: 'demo@diario.local', senha: '123456' });
  const [notice, setNotice] = useState('');
  const [loading, setLoading] = useState(false);

  const api = useMemo(() => createApiClient({
    token,
    onUnauthorized: () => {
      localStorage.removeItem(tokenKey);
      setToken(null);
      setUser(null);
    }
  }), [token]);

  const selectedEmotion = useMemo(
    () => catalog.find((emotion) => emotion.id === entryForm.emocaoId),
    [catalog, entryForm.emocaoId]
  );

  async function loadCatalog() {
    setCatalog(await api.request('/catalogo/emocoes'));
  }

  async function loadPrivateData() {
    if (!token) return;
    const [me, newEntries, newDashboard, newReport, newCrisis] = await Promise.all([
      api.request('/auth/me'),
      api.request('/registros'),
      api.request('/dashboard'),
      api.request('/relatorios/semanal'),
      api.request('/crise')
    ]);
    setUser(me);
    setEntries(newEntries);
    setDashboard(newDashboard);
    setReport(newReport);
    setCrisis(newCrisis);
  }

  async function runAction(action, successMessage) {
    setLoading(true);
    setNotice('');
    try {
      const result = await action();
      if (successMessage) setNotice(successMessage);
      return result;
    } catch (error) {
      setNotice(error.message);
      return null;
    } finally {
      setLoading(false);
    }
  }

  async function authenticate(event) {
    event.preventDefault();
    await runAction(async () => {
      const path = authMode === 'login' ? '/auth/login' : '/auth/register';
      const payload = authMode === 'login'
        ? { email: authForm.email, senha: authForm.senha }
        : authForm;
      const data = await api.request(path, { method: 'POST', body: JSON.stringify(payload) });
      localStorage.setItem(tokenKey, data.token);
      setToken(data.token);
      setUser(data.usuario);
    }, 'Acesso realizado com segurança.');
  }

  async function logout() {
    await runAction(async () => {
      await api.request('/auth/logout', { method: 'POST' });
      localStorage.removeItem(tokenKey);
      setToken(null);
      setUser(null);
      setEntries([]);
      setDashboard(null);
      setReport(null);
    });
  }

  async function saveEntry(event) {
    event.preventDefault();
    await runAction(async () => {
      await api.request('/registros', { method: 'POST', body: JSON.stringify(entryForm) });
      setEntryForm({ ...emptyEntryForm, emocaoId: entryForm.emocaoId });
      await loadPrivateData();
    }, 'Registro emocional salvo.');
  }

  async function removeEntry(id) {
    await runAction(async () => {
      await api.request(`/registros/${id}`, { method: 'DELETE' });
      await loadPrivateData();
    }, 'Registro removido.');
  }

  async function updateEsm(nextConfig) {
    await runAction(async () => {
      const esm = await api.request('/esm/configuracao', { method: 'PUT', body: JSON.stringify(nextConfig) });
      setUser((current) => ({ ...current, esm }));
      await loadPrivateData();
    }, 'Configuração ESM atualizada.');
  }

  async function updatePrivacy(nextConfig) {
    await runAction(async () => {
      const privacidade = await api.request('/privacidade', { method: 'PUT', body: JSON.stringify(nextConfig) });
      setUser((current) => ({ ...current, privacidade }));
    }, 'Preferências de privacidade salvas.');
  }

  async function deleteData() {
    await runAction(async () => {
      const result = await api.request('/privacidade/dados', { method: 'DELETE' });
      await loadPrivateData();
      setNotice(`${result.registrosRemovidos} registros excluídos.`);
    });
  }

  async function exportPdf() {
    await runAction(async () => {
      const blob = await api.download('/relatorios/pdf');
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = 'relatorio-emocional.pdf';
      link.click();
      URL.revokeObjectURL(url);
    });
  }

  useEffect(() => {
    loadCatalog().catch((error) => setNotice(error.message));
  }, [api]);

  useEffect(() => {
    loadPrivateData().catch((error) => setNotice(error.message));
  }, [token]);

  if (!token) {
    return (
      <AuthPage
        mode={authMode}
        form={authForm}
        loading={loading}
        notice={notice}
        onModeChange={setAuthMode}
        onFormChange={setAuthForm}
        onSubmit={authenticate}
      />
    );
  }

  return (
    <main className="app-shell">
      <AppHeader userName={user?.nome} activeTab={activeTab} onTabChange={setActiveTab} onLogout={logout} />
      {notice && <div className="toast">{notice}</div>}
      <DashboardMetrics dashboard={dashboard} report={report} />

      {activeTab === 'diario' && (
        <section className="workspace two-columns">
          <EntryForm
            catalog={catalog}
            form={entryForm}
            selectedEmotion={selectedEmotion}
            loading={loading}
            onFormChange={setEntryForm}
            onSubmit={saveEntry}
          />
          <SupportPanel selectedEmotion={selectedEmotion} suggestion={dashboard?.sugestaoRapida} crisis={crisis} />
        </section>
      )}

      {activeTab === 'historico' && (
        <TimelineView entries={entries} loading={loading} onRemove={removeEntry} />
      )}

      {activeTab === 'relatorio' && (
        <ReportView report={report} loading={loading} onExportPdf={exportPdf} />
      )}

      {activeTab === 'configuracoes' && (
        <SettingsView
          user={user}
          loading={loading}
          onUpdateEsm={updateEsm}
          onUpdatePrivacy={updatePrivacy}
          onDeleteData={deleteData}
        />
      )}
    </main>
  );
}
