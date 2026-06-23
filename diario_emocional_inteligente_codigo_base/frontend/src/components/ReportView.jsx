import React from 'react';

export function ReportView({ report, loading, onExportPdf }) {
  return (
    <section className="workspace two-columns">
      <div>
        <div className="section-heading">
          <p className="eyebrow">Últimos 7 dias</p>
          <h2>Relatório visual</h2>
        </div>
        <div className="report-bars">
          {(report?.distribuicaoEmocional ?? []).map((item) => (
            <Bar
              key={item.nome}
              label={item.nome}
              value={item.quantidade}
              max={Math.max(...report.distribuicaoEmocional.map((entry) => entry.quantidade), 1)}
            />
          ))}
          {(report?.distribuicaoEmocional?.length ?? 0) === 0 && <p className="empty">Sem dados na semana.</p>}
        </div>
        <button className="primary" onClick={onExportPdf} disabled={loading}>Exportar PDF</button>
      </div>
      <aside className="support-panel">
        <h2>{report?.emocaoMaisFrequente ?? 'sem dados'}</h2>
        <p>{report?.insightPrincipal}</p>
        <div className="trigger-list">
          {(report?.gatilhosFrequentes ?? []).map((item) => (
            <span key={item.nome}>{item.nome} · {item.quantidade}</span>
          ))}
        </div>
      </aside>
    </section>
  );
}

function Bar({ label, value, max }) {
  return (
    <div className="bar-row">
      <span>{label}</span>
      <div className="bar-track">
        <div style={{ width: `${Math.max((value / max) * 100, 8)}%` }} />
      </div>
      <strong>{value}</strong>
    </div>
  );
}
