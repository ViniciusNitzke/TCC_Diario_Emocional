import React from 'react';
import { navItems } from '../constants/emotionalCatalog.js';

export function AppHeader({ userName, activeTab, onTabChange, onLogout }) {
  return (
    <header className="topbar">
      <div>
        <p className="eyebrow">Diário emocional</p>
        <h1>{userName ?? 'Usuário'}</h1>
      </div>
      <nav className="tabs" aria-label="Navegação principal">
        {navItems.map(([id, label]) => (
          <button key={id} className={activeTab === id ? 'active' : ''} onClick={() => onTabChange(id)}>{label}</button>
        ))}
      </nav>
      <button className="ghost" onClick={onLogout}>Sair</button>
    </header>
  );
}
