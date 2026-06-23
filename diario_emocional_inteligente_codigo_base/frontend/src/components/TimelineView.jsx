import React from 'react';

export function TimelineView({ entries, loading, onRemove }) {
  return (
    <section className="workspace">
      <div className="section-heading">
        <p className="eyebrow">Linha do tempo</p>
        <h2>Histórico emocional</h2>
      </div>
      <div className="timeline">
        {entries.length === 0 && <p className="empty">Nenhum registro salvo ainda.</p>}
        {entries.map((entry) => (
          <article key={entry.id} className="timeline-item">
            <div>
              <strong>{entry.emocaoNome}</strong>
              <span>{new Date(entry.criadoEm).toLocaleString('pt-BR')}</span>
            </div>
            <p>{entry.situacao || entry.observacao || 'Registro sem nota textual.'}</p>
            <div className="timeline-meta">
              <span>Intensidade {entry.intensidade}/5</span>
              <span>{entry.gatilhos.join(', ') || 'sem gatilho'}</span>
              <span>{entry.origem}</span>
              <button onClick={() => onRemove(entry.id)} disabled={loading}>Remover</button>
            </div>
          </article>
        ))}
      </div>
    </section>
  );
}
