import React from 'react';

export function SettingsView({ user, loading, onUpdateEsm, onUpdatePrivacy, onDeleteData }) {
  if (!user) return null;

  return (
    <section className="workspace settings-grid">
      <SettingsPanel
        title="Lembretes ESM"
        items={[
          ['Ativo', user.esm.ativo],
          ['Frequência', user.esm.frequencia],
          ['Horários', user.esm.horarios.join(', ')]
        ]}
      >
        <label className="switch">
          <input
            type="checkbox"
            checked={user.esm.ativo}
            onChange={(event) => onUpdateEsm({ ...user.esm, ativo: event.target.checked })}
          />
          <span>Coleta programada</span>
        </label>
        <button onClick={() => onUpdateEsm({ ...user.esm, horarios: ['08:30', '13:30', '19:30'] })}>Aplicar rotina padrão</button>
      </SettingsPanel>

      <SettingsPanel
        title="Privacidade"
        items={[
          ['Bloqueio local', user.privacidade.bloqueioLocal],
          ['Relatórios', user.privacidade.permitirRelatorios],
          ['Consentimento', user.privacidade.consentimentoDadosSensiveis]
        ]}
      >
        {Object.entries(user.privacidade).map(([key, value]) => (
          <label className="switch" key={key}>
            <input
              type="checkbox"
              checked={value}
              onChange={(event) => onUpdatePrivacy({ ...user.privacidade, [key]: event.target.checked })}
            />
            <span>{privacyLabel(key)}</span>
          </label>
        ))}
        <button className="danger" onClick={onDeleteData} disabled={loading}>Excluir registros emocionais</button>
      </SettingsPanel>
    </section>
  );
}

function SettingsPanel({ title, items, children }) {
  return (
    <article className="settings-panel">
      <h2>{title}</h2>
      <dl>
        {items.map(([label, value]) => (
          <div key={label}>
            <dt>{label}</dt>
            <dd>{typeof value === 'boolean' ? (value ? 'Sim' : 'Não') : value}</dd>
          </div>
        ))}
      </dl>
      <div className="settings-actions">{children}</div>
    </article>
  );
}

function privacyLabel(key) {
  return {
    bloqueioLocal: 'Bloqueio local',
    permitirRelatorios: 'Relatórios visuais',
    consentimentoDadosSensiveis: 'Consentimento LGPD'
  }[key] ?? key;
}
