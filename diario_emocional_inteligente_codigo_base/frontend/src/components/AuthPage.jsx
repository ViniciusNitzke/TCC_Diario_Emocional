import React from 'react';

export function AuthPage({ mode, form, loading, notice, onModeChange, onFormChange, onSubmit }) {
  return (
    <main className="auth-page">
      <section className="auth-panel">
        <div>
          <p className="eyebrow">Saúde mental e Engenharia de Software</p>
          <h1>Diário Emocional Inteligente</h1>
          <p className="auth-copy">
            Registros ESM, análise de padrões, autocuidado e privacidade em uma aplicação web funcional.
          </p>
        </div>

        <form className="auth-form" onSubmit={onSubmit}>
          <div className="segmented">
            <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => onModeChange('login')}>Entrar</button>
            <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => onModeChange('register')}>Criar conta</button>
          </div>
          {mode === 'register' && (
            <label>
              Nome
              <input value={form.nome} onChange={(event) => onFormChange({ ...form, nome: event.target.value })} />
            </label>
          )}
          <label>
            E-mail
            <input type="email" value={form.email} onChange={(event) => onFormChange({ ...form, email: event.target.value })} />
          </label>
          <label>
            Senha
            <input type="password" value={form.senha} onChange={(event) => onFormChange({ ...form, senha: event.target.value })} />
          </label>
          <button className="primary" disabled={loading}>{loading ? 'Aguarde...' : mode === 'login' ? 'Entrar' : 'Cadastrar'}</button>
          {notice && <p className="notice">{notice}</p>}
        </form>
      </section>
    </main>
  );
}
