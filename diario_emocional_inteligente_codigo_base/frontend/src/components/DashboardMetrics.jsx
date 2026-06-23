import React from 'react';

export function DashboardMetrics({ dashboard, report }) {
  return (
    <section className="dashboard-grid">
      <Metric title="Hoje" value={dashboard?.registrosHoje ?? 0} detail={dashboard?.humorHoje ?? 'sem registro'} />
      <Metric title="Sequência" value={`${dashboard?.sequenciaDias ?? 0}d`} detail="dias com check-in" />
      <Metric title="ESM" value={dashboard?.proximoPromptEsm ?? '--'} detail="próximo lembrete" />
      <Metric title="Semana" value={report?.intensidadeMedia ?? 0} detail="intensidade média" />
    </section>
  );
}

function Metric({ title, value, detail }) {
  return (
    <article className="metric">
      <span>{title}</span>
      <strong>{value}</strong>
      <small>{detail}</small>
    </article>
  );
}
