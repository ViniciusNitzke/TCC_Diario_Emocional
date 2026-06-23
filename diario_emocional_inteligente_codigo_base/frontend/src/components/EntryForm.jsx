import React from 'react';
import { triggerOptions } from '../constants/emotionalCatalog.js';

export function EntryForm({ catalog, form, selectedEmotion, loading, onFormChange, onSubmit }) {
  return (
    <form className="entry-panel" onSubmit={onSubmit}>
      <div className="section-heading">
        <p className="eyebrow">Check-in</p>
        <h2>Registro de agora</h2>
      </div>

      <div className="emotion-grid">
        {catalog.map((emotion) => (
          <button
            type="button"
            key={emotion.id}
            className={form.emocaoId === emotion.id ? 'emotion active' : 'emotion'}
            style={{ '--emotion-color': emotion.cor }}
            onClick={() => onFormChange({ ...form, emocaoId: emotion.id })}
            title={emotion.descricao}
          >
            <span />
            {emotion.nome}
          </button>
        ))}
      </div>

      <label>
        Intensidade emocional
        <input
          type="range"
          min="1"
          max="5"
          value={form.intensidade}
          onChange={(event) => onFormChange({ ...form, intensidade: Number(event.target.value) })}
        />
        <strong className="range-value">{form.intensidade}/5</strong>
      </label>

      <div className="chips" aria-label="Gatilhos percebidos">
        {triggerOptions.map((trigger) => {
          const active = form.gatilhos.includes(trigger);
          return (
            <button
              type="button"
              key={trigger}
              className={active ? 'chip active' : 'chip'}
              onClick={() => onFormChange({
                ...form,
                gatilhos: active
                  ? form.gatilhos.filter((item) => item !== trigger)
                  : [...form.gatilhos, trigger]
              })}
            >
              {trigger}
            </button>
          );
        })}
      </div>

      <label>
        Situação
        <input value={form.situacao} onChange={(event) => onFormChange({ ...form, situacao: event.target.value })} />
      </label>
      <label>
        Nota pessoal
        <textarea value={form.observacao} onChange={(event) => onFormChange({ ...form, observacao: event.target.value })} />
      </label>
      <input type="hidden" value={selectedEmotion?.nome ?? ''} readOnly />
      <button className="primary" disabled={loading}>Salvar registro</button>
    </form>
  );
}
