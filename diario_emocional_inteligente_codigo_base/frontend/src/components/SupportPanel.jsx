export function SupportPanel({ selectedEmotion, suggestion, crisis }) {
  return (
    <aside className="support-panel">
      <div className="section-heading">
        <p className="eyebrow">{selectedEmotion?.familia ?? 'regulação'}</p>
        <h2>{selectedEmotion?.nome ?? 'Emoção'}</h2>
      </div>
      <p>{selectedEmotion?.descricao}</p>
      <div className="suggestion">
        <strong>{suggestion?.titulo ?? 'Pausa consciente'}</strong>
        <span>{suggestion?.descricao ?? 'Respire e observe seu corpo por um minuto.'}</span>
      </div>
      {crisis && (
        <div className="crisis-box">
          <h3>{crisis.titulo}</h3>
          <p>{crisis.aviso}</p>
          {crisis.passos.map((step) => (
            <details key={step.titulo}>
              <summary>{step.titulo}</summary>
              <p>{step.descricao}</p>
            </details>
          ))}
        </div>
      )}
    </aside>
  );
}
