import { useState, useEffect, useCallback, useMemo } from "react";

// ─── Data Models & Mock Data ────────────────────────────────────────
const TINACO_MODELS = [
  { id: "T-450", name: "Tinaco 450L", capacity: 450, color: "Negro", layers: 3, weight: 8.5 },
  { id: "T-750", name: "Tinaco 750L", capacity: 750, color: "Negro", layers: 3, weight: 12.2 },
  { id: "T-1100", name: "Tinaco 1100L", capacity: 1100, color: "Negro", layers: 3, weight: 16.8 },
  { id: "T-2500", name: "Tinaco 2500L", capacity: 2500, color: "Negro", layers: 3, weight: 28.5 },
  { id: "T-5000", name: "Tinaco 5000L", capacity: 5000, color: "Negro", layers: 3, weight: 45.0 },
];

const RAW_MATERIALS = [
  { id: "RM-PE-HD", name: "Polietileno HD (Virgen)", unit: "kg", stock: 12500, minStock: 5000, cost: 28.50, category: "resina" },
  { id: "RM-PE-R", name: "Polietileno Reciclado", unit: "kg", stock: 8200, minStock: 3000, cost: 15.00, category: "resina" },
  { id: "RM-MB-N", name: "Masterbatch Negro UV", unit: "kg", stock: 450, minStock: 200, cost: 85.00, category: "aditivo" },
  { id: "RM-MB-B", name: "Masterbatch Blanco", unit: "kg", stock: 320, minStock: 150, cost: 92.00, category: "aditivo" },
  { id: "RM-EST", name: "Estabilizador UV", unit: "kg", stock: 180, minStock: 100, cost: 120.00, category: "aditivo" },
  { id: "RM-CONN-1", name: "Conector 1/2\" PVC", unit: "pz", stock: 2400, minStock: 1000, cost: 8.50, category: "accesorio" },
  { id: "RM-CONN-2", name: "Conector 3/4\" PVC", unit: "pz", stock: 1800, minStock: 800, cost: 12.00, category: "accesorio" },
  { id: "RM-FLOT", name: "Flotador completo", unit: "pz", stock: 950, minStock: 400, cost: 45.00, category: "accesorio" },
  { id: "RM-TAPA", name: "Tapa con cierre", unit: "pz", stock: 1100, minStock: 500, cost: 22.00, category: "accesorio" },
  { id: "RM-BASE", name: "Base reforzada", unit: "pz", stock: 600, minStock: 300, cost: 35.00, category: "accesorio" },
  { id: "RM-ETIQ", name: "Etiqueta/Calcomanía", unit: "pz", stock: 3500, minStock: 1500, cost: 3.50, category: "empaque" },
  { id: "RM-FILM", name: "Película stretch", unit: "rollo", stock: 45, minStock: 20, cost: 280.00, category: "empaque" },
];

const BOM = {
  "T-450": [
    { materialId: "RM-PE-HD", qty: 4.5 }, { materialId: "RM-PE-R", qty: 2.0 },
    { materialId: "RM-MB-N", qty: 0.3 }, { materialId: "RM-EST", qty: 0.15 },
    { materialId: "RM-CONN-1", qty: 1 }, { materialId: "RM-FLOT", qty: 1 },
    { materialId: "RM-TAPA", qty: 1 }, { materialId: "RM-ETIQ", qty: 1 },
  ],
  "T-750": [
    { materialId: "RM-PE-HD", qty: 6.5 }, { materialId: "RM-PE-R", qty: 3.5 },
    { materialId: "RM-MB-N", qty: 0.45 }, { materialId: "RM-EST", qty: 0.2 },
    { materialId: "RM-CONN-1", qty: 1 }, { materialId: "RM-CONN-2", qty: 1 },
    { materialId: "RM-FLOT", qty: 1 }, { materialId: "RM-TAPA", qty: 1 },
    { materialId: "RM-BASE", qty: 1 }, { materialId: "RM-ETIQ", qty: 1 },
  ],
  "T-1100": [
    { materialId: "RM-PE-HD", qty: 9.0 }, { materialId: "RM-PE-R", qty: 5.0 },
    { materialId: "RM-MB-N", qty: 0.6 }, { materialId: "RM-EST", qty: 0.3 },
    { materialId: "RM-CONN-1", qty: 1 }, { materialId: "RM-CONN-2", qty: 1 },
    { materialId: "RM-FLOT", qty: 1 }, { materialId: "RM-TAPA", qty: 1 },
    { materialId: "RM-BASE", qty: 1 }, { materialId: "RM-ETIQ", qty: 1 },
  ],
  "T-2500": [
    { materialId: "RM-PE-HD", qty: 16.0 }, { materialId: "RM-PE-R", qty: 8.0 },
    { materialId: "RM-MB-N", qty: 1.0 }, { materialId: "RM-MB-B", qty: 0.5 },
    { materialId: "RM-EST", qty: 0.5 }, { materialId: "RM-CONN-2", qty: 2 },
    { materialId: "RM-FLOT", qty: 1 }, { materialId: "RM-TAPA", qty: 1 },
    { materialId: "RM-BASE", qty: 1 }, { materialId: "RM-ETIQ", qty: 2 },
  ],
  "T-5000": [
    { materialId: "RM-PE-HD", qty: 28.0 }, { materialId: "RM-PE-R", qty: 12.0 },
    { materialId: "RM-MB-N", qty: 1.8 }, { materialId: "RM-MB-B", qty: 0.8 },
    { materialId: "RM-EST", qty: 0.8 }, { materialId: "RM-CONN-2", qty: 2 },
    { materialId: "RM-FLOT", qty: 1 }, { materialId: "RM-TAPA", qty: 1 },
    { materialId: "RM-BASE", qty: 1 }, { materialId: "RM-ETIQ", qty: 2 },
    { materialId: "RM-FILM", qty: 0.5 },
  ],
};

const MACHINES = [
  { id: "ROT-01", name: "Rotomoldeadora #1", type: "rotomoldeo", status: "running", currentModel: "T-1100", cycleTime: 22, temp: 285, rpm: 8.5 },
  { id: "ROT-02", name: "Rotomoldeadora #2", type: "rotomoldeo", status: "running", currentModel: "T-750", cycleTime: 18, temp: 280, rpm: 9.0 },
  { id: "ROT-03", name: "Rotomoldeadora #3", type: "rotomoldeo", status: "idle", currentModel: null, cycleTime: 0, temp: 25, rpm: 0 },
  { id: "ROT-04", name: "Rotomoldeadora #4", type: "rotomoldeo", status: "maintenance", currentModel: null, cycleTime: 0, temp: 25, rpm: 0 },
  { id: "ENF-01", name: "Estación Enfriamiento #1", type: "enfriamiento", status: "running" },
  { id: "REB-01", name: "Desbarbadora", type: "rebabeo", status: "running" },
  { id: "ENS-01", name: "Estación Ensamble #1", type: "ensamble", status: "running" },
  { id: "ENS-02", name: "Estación Ensamble #2", type: "ensamble", status: "idle" },
  { id: "PRU-01", name: "Estación Prueba Hermeticidad", type: "prueba", status: "running" },
];

function generateProductionOrders() {
  const statuses = ["pending", "in_progress", "molding", "cooling", "trimming", "assembly", "testing", "completed"];
  const orders = [];
  for (let i = 1; i <= 18; i++) {
    const model = TINACO_MODELS[Math.floor(Math.random() * TINACO_MODELS.length)];
    const qty = Math.floor(Math.random() * 50) + 10;
    const completed = Math.floor(Math.random() * qty);
    const status = statuses[Math.floor(Math.random() * statuses.length)];
    orders.push({
      id: `OP-2026-${String(i).padStart(4, "0")}`,
      model: model.id,
      modelName: model.name,
      qty,
      completed,
      status,
      startDate: `2026-02-${String(Math.floor(Math.random() * 8) + 1).padStart(2, "0")}`,
      priority: ["alta", "media", "baja"][Math.floor(Math.random() * 3)],
      machine: status === "molding" ? MACHINES[Math.floor(Math.random() * 2)].id : null,
    });
  }
  return orders;
}

function generateAssemblyLog() {
  const logs = [];
  for (let i = 1; i <= 25; i++) {
    const model = TINACO_MODELS[Math.floor(Math.random() * TINACO_MODELS.length)];
    logs.push({
      id: `ASM-${String(i).padStart(4, "0")}`,
      tinacoSerial: `SN-2026-${String(Math.floor(Math.random() * 9000) + 1000)}`,
      model: model.id,
      modelName: model.name,
      operator: ["J. García", "M. López", "R. Hernández", "A. Martínez", "P. Sánchez"][Math.floor(Math.random() * 5)],
      station: Math.random() > 0.5 ? "ENS-01" : "ENS-02",
      steps: {
        tapa: true,
        conectores: true,
        flotador: Math.random() > 0.1,
        base: model.capacity >= 750 ? Math.random() > 0.05 : false,
        etiqueta: Math.random() > 0.15,
        pruebaHermeticidad: Math.random() > 0.08,
      },
      timestamp: `2026-02-0${Math.floor(Math.random() * 8) + 1}T${String(Math.floor(Math.random() * 10) + 6).padStart(2, "0")}:${String(Math.floor(Math.random() * 60)).padStart(2, "0")}`,
      duration: Math.floor(Math.random() * 20) + 5,
      defects: Math.random() > 0.85 ? ["burbuja", "deformación", "rebaba excesiva", "fuga"][Math.floor(Math.random() * 4)] : null,
    });
  }
  return logs;
}

function generateProcessData() {
  const data = [];
  for (let i = 0; i < 48; i++) {
    const hour = Math.floor(i / 2);
    const min = (i % 2) * 30;
    data.push({
      time: `${String(hour).padStart(2, "0")}:${String(min).padStart(2, "0")}`,
      ovenTemp1: 275 + Math.random() * 20,
      ovenTemp2: 278 + Math.random() * 18,
      rpm1: 8 + Math.random() * 2,
      rpm2: 8.5 + Math.random() * 1.5,
      coolingTemp: 40 + Math.random() * 15,
      wallThickness: 4.8 + Math.random() * 0.8,
      cycleTime: 18 + Math.random() * 8,
    });
  }
  return data;
}

const PRODUCTION_ORDERS = generateProductionOrders();
const ASSEMBLY_LOG = generateAssemblyLog();
const PROCESS_DATA = generateProcessData();

// ─── Finished Goods Inventory ────────────────────────────────────────
const FINISHED_GOODS = TINACO_MODELS.map(m => ({
  ...m,
  inStock: Math.floor(Math.random() * 80) + 10,
  reserved: Math.floor(Math.random() * 20),
  minStock: [15, 12, 10, 5, 3][TINACO_MODELS.indexOf(m)],
}));

// ─── Icons ──────────────────────────────────────────────────────────
const Icons = {
  Dashboard: () => <svg width="20" height="20" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/></svg>,
  Inventory: () => <svg width="20" height="20" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"/></svg>,
  Production: () => <svg width="20" height="20" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.066 2.573c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.573 1.066c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.066-2.573c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/><circle cx="12" cy="12" r="3"/></svg>,
  Assembly: () => <svg width="20" height="20" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>,
  Process: () => <svg width="20" height="20" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/></svg>,
  Alert: () => <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path d="M12 9v2m0 4h.01M10.29 3.86l-8.58 14.86A1 1 0 002.59 20h16.82a1 1 0 00.87-1.5L12.71 3.86a1 1 0 00-1.74 0z"/></svg>,
  Check: () => <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path d="M5 13l4 4L19 7"/></svg>,
  X: () => <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path d="M6 18L18 6M6 6l12 12"/></svg>,
  Clock: () => <svg width="14" height="14" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/></svg>,
  Droplet: () => <svg width="48" height="48" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}><path d="M12 2.69l5.66 5.66a8 8 0 11-11.31 0z"/></svg>,
};

// ─── Utility Components ─────────────────────────────────────────────
const StatusBadge = ({ status }) => {
  const colors = {
    running: { bg: "#0d3320", text: "#34d399", border: "#065f46" },
    idle: { bg: "#332800", text: "#fbbf24", border: "#78350f" },
    maintenance: { bg: "#3b1018", text: "#f87171", border: "#7f1d1d" },
    completed: { bg: "#0d3320", text: "#34d399", border: "#065f46" },
    in_progress: { bg: "#172554", text: "#60a5fa", border: "#1e3a5f" },
    molding: { bg: "#3b1a54", text: "#c084fc", border: "#581c87" },
    cooling: { bg: "#083344", text: "#22d3ee", border: "#164e63" },
    trimming: { bg: "#332800", text: "#fbbf24", border: "#78350f" },
    assembly: { bg: "#172554", text: "#60a5fa", border: "#1e3a5f" },
    testing: { bg: "#3b1a54", text: "#c084fc", border: "#581c87" },
    pending: { bg: "#1c1c1c", text: "#a1a1a1", border: "#333" },
    alta: { bg: "#3b1018", text: "#f87171", border: "#7f1d1d" },
    media: { bg: "#332800", text: "#fbbf24", border: "#78350f" },
    baja: { bg: "#0d3320", text: "#34d399", border: "#065f46" },
  };
  const c = colors[status] || colors.pending;
  const labels = {
    running: "Activa", idle: "Inactiva", maintenance: "Mantenimiento",
    completed: "Completada", in_progress: "En Proceso", molding: "Moldeo",
    cooling: "Enfriamiento", trimming: "Rebabeo", assembly: "Ensamble",
    testing: "Prueba", pending: "Pendiente", alta: "Alta", media: "Media", baja: "Baja",
  };
  return (
    <span style={{
      display: "inline-block", padding: "2px 10px", borderRadius: 999,
      fontSize: 11, fontWeight: 600, letterSpacing: 0.5,
      background: c.bg, color: c.text, border: `1px solid ${c.border}`,
      textTransform: "uppercase",
    }}>
      {labels[status] || status}
    </span>
  );
};

const Card = ({ title, children, accent, style }) => (
  <div style={{
    background: "#111214", borderRadius: 12, padding: "20px 24px",
    border: "1px solid #222", position: "relative", overflow: "hidden", ...style,
  }}>
    {accent && <div style={{ position: "absolute", top: 0, left: 0, right: 0, height: 3, background: accent }} />}
    {title && <h3 style={{ margin: "0 0 16px", fontSize: 14, fontWeight: 600, color: "#a1a1a1", textTransform: "uppercase", letterSpacing: 1 }}>{title}</h3>}
    {children}
  </div>
);

const MiniChart = ({ data, color = "#60a5fa", height = 40, width = 120 }) => {
  if (!data || data.length === 0) return null;
  const max = Math.max(...data);
  const min = Math.min(...data);
  const range = max - min || 1;
  const points = data.map((v, i) => `${(i / (data.length - 1)) * width},${height - ((v - min) / range) * height}`).join(" ");
  return (
    <svg width={width} height={height} style={{ display: "block" }}>
      <polyline points={points} fill="none" stroke={color} strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  );
};

const ProgressBar = ({ value, max, color = "#60a5fa", height = 6 }) => (
  <div style={{ background: "#1c1c1c", borderRadius: height / 2, height, overflow: "hidden", width: "100%" }}>
    <div style={{ width: `${Math.min((value / max) * 100, 100)}%`, height: "100%", background: color, borderRadius: height / 2, transition: "width 0.5s ease" }} />
  </div>
);

// ─── Dashboard Module ───────────────────────────────────────────────
const DashboardView = () => {
  const totalProduction = PRODUCTION_ORDERS.reduce((s, o) => s + o.completed, 0);
  const totalTarget = PRODUCTION_ORDERS.reduce((s, o) => s + o.qty, 0);
  const activeOrders = PRODUCTION_ORDERS.filter(o => !["completed", "pending"].includes(o.status)).length;
  const lowStockItems = RAW_MATERIALS.filter(m => m.stock <= m.minStock * 1.2).length;
  const defectRate = ((ASSEMBLY_LOG.filter(a => a.defects).length / ASSEMBLY_LOG.length) * 100).toFixed(1);
  const activeMachines = MACHINES.filter(m => m.status === "running").length;
  const finishedTotal = FINISHED_GOODS.reduce((s, g) => s + g.inStock, 0);

  const kpis = [
    { label: "Producción Hoy", value: totalProduction, sub: `de ${totalTarget} objetivo`, color: "#60a5fa" },
    { label: "Órdenes Activas", value: activeOrders, sub: `de ${PRODUCTION_ORDERS.length} totales`, color: "#c084fc" },
    { label: "Máquinas Activas", value: `${activeMachines}/${MACHINES.length}`, sub: "rotomoldeadoras + estaciones", color: "#34d399" },
    { label: "Tasa Defectos", value: `${defectRate}%`, sub: "últimas 25 unidades", color: parseFloat(defectRate) > 10 ? "#f87171" : "#34d399" },
    { label: "Inventario PT", value: finishedTotal, sub: "tinacos terminados", color: "#fbbf24" },
    { label: "Alertas Inventario", value: lowStockItems, sub: "materiales bajo mínimo", color: lowStockItems > 0 ? "#f87171" : "#34d399" },
  ];

  return (
    <div>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(200px, 1fr))", gap: 16, marginBottom: 24 }}>
        {kpis.map((k, i) => (
          <Card key={i}>
            <div style={{ fontSize: 12, color: "#666", marginBottom: 8, fontWeight: 500 }}>{k.label}</div>
            <div style={{ fontSize: 28, fontWeight: 700, color: k.color, fontFamily: "'JetBrains Mono', monospace" }}>{k.value}</div>
            <div style={{ fontSize: 11, color: "#555", marginTop: 4 }}>{k.sub}</div>
          </Card>
        ))}
      </div>

      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginBottom: 24 }}>
        <Card title="Estado de Máquinas" accent="#60a5fa">
          <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            {MACHINES.filter(m => m.type === "rotomoldeo").map(m => (
              <div key={m.id} style={{ display: "flex", alignItems: "center", gap: 12, padding: "8px 12px", background: "#0a0a0b", borderRadius: 8, border: "1px solid #1a1a1a" }}>
                <div style={{ width: 8, height: 8, borderRadius: "50%", background: m.status === "running" ? "#34d399" : m.status === "idle" ? "#fbbf24" : "#f87171", boxShadow: m.status === "running" ? "0 0 8px #34d39966" : "none" }} />
                <div style={{ flex: 1 }}>
                  <div style={{ fontSize: 13, fontWeight: 600, color: "#e5e5e5" }}>{m.name}</div>
                  <div style={{ fontSize: 11, color: "#666" }}>
                    {m.currentModel ? `Modelo: ${m.currentModel} · ${m.temp}°C · ${m.rpm} RPM` : m.status === "maintenance" ? "En mantenimiento programado" : "Sin orden asignada"}
                  </div>
                </div>
                <StatusBadge status={m.status} />
              </div>
            ))}
          </div>
        </Card>

        <Card title="Producción por Modelo" accent="#c084fc">
          {TINACO_MODELS.map(model => {
            const orders = PRODUCTION_ORDERS.filter(o => o.model === model.id);
            const produced = orders.reduce((s, o) => s + o.completed, 0);
            const target = orders.reduce((s, o) => s + o.qty, 0);
            return (
              <div key={model.id} style={{ marginBottom: 14 }}>
                <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 4 }}>
                  <span style={{ fontSize: 13, color: "#ccc", fontWeight: 500 }}>{model.name}</span>
                  <span style={{ fontSize: 12, color: "#888", fontFamily: "'JetBrains Mono', monospace" }}>{produced}/{target}</span>
                </div>
                <ProgressBar value={produced} max={target || 1} color={["#60a5fa", "#c084fc", "#34d399", "#fbbf24", "#f87171"][TINACO_MODELS.indexOf(model)]} />
              </div>
            );
          })}
        </Card>
      </div>

      <Card title="Alertas Activas" accent="#f87171">
        <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
          {RAW_MATERIALS.filter(m => m.stock <= m.minStock * 1.2).map(m => (
            <div key={m.id} style={{ display: "flex", alignItems: "center", gap: 10, padding: "8px 12px", background: "#1a0a0a", borderRadius: 8, border: "1px solid #3b1018" }}>
              <span style={{ color: "#f87171" }}><Icons.Alert /></span>
              <span style={{ fontSize: 13, color: "#f87171", flex: 1 }}>
                <strong>{m.name}</strong>: {m.stock} {m.unit} (mín: {m.minStock})
              </span>
              <span style={{ fontSize: 11, color: "#f8717188" }}>
                {((m.stock / m.minStock) * 100).toFixed(0)}% del mínimo
              </span>
            </div>
          ))}
          {MACHINES.filter(m => m.status === "maintenance").map(m => (
            <div key={m.id} style={{ display: "flex", alignItems: "center", gap: 10, padding: "8px 12px", background: "#1a1300", borderRadius: 8, border: "1px solid #78350f" }}>
              <span style={{ color: "#fbbf24" }}><Icons.Alert /></span>
              <span style={{ fontSize: 13, color: "#fbbf24" }}><strong>{m.name}</strong>: En mantenimiento</span>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
};

// ─── Inventory Module ───────────────────────────────────────────────
const InventoryView = () => {
  const [tab, setTab] = useState("raw");
  const [filter, setFilter] = useState("");

  const filteredRaw = RAW_MATERIALS.filter(m =>
    m.name.toLowerCase().includes(filter.toLowerCase()) || m.id.toLowerCase().includes(filter.toLowerCase())
  );

  return (
    <div>
      <div style={{ display: "flex", gap: 8, marginBottom: 20 }}>
        {[["raw", "Materia Prima"], ["finished", "Producto Terminado"], ["bom", "Lista de Materiales"]].map(([key, label]) => (
          <button key={key} onClick={() => setTab(key)} style={{
            padding: "8px 20px", borderRadius: 8, border: "1px solid",
            borderColor: tab === key ? "#60a5fa" : "#333",
            background: tab === key ? "#172554" : "#111214",
            color: tab === key ? "#60a5fa" : "#888",
            cursor: "pointer", fontSize: 13, fontWeight: 600, transition: "all 0.2s",
          }}>{label}</button>
        ))}
      </div>

      {tab === "raw" && (
        <>
          <input
            type="text" placeholder="Buscar material..." value={filter}
            onChange={e => setFilter(e.target.value)}
            style={{ width: "100%", padding: "10px 16px", background: "#0a0a0b", border: "1px solid #222", borderRadius: 8, color: "#e5e5e5", fontSize: 13, marginBottom: 16, outline: "none", boxSizing: "border-box" }}
          />
          <div style={{ overflowX: "auto" }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ borderBottom: "1px solid #222" }}>
                  {["Código", "Material", "Categoría", "Stock", "Mínimo", "Estado", "Costo Unit.", "Valor Total"].map(h => (
                    <th key={h} style={{ padding: "10px 12px", textAlign: "left", color: "#666", fontWeight: 600, fontSize: 11, textTransform: "uppercase", letterSpacing: 0.5 }}>{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {filteredRaw.map(m => {
                  const ratio = m.stock / m.minStock;
                  return (
                    <tr key={m.id} style={{ borderBottom: "1px solid #1a1a1a" }}>
                      <td style={{ padding: "10px 12px", fontFamily: "'JetBrains Mono', monospace", color: "#888", fontSize: 12 }}>{m.id}</td>
                      <td style={{ padding: "10px 12px", color: "#e5e5e5", fontWeight: 500 }}>{m.name}</td>
                      <td style={{ padding: "10px 12px" }}>
                        <span style={{ padding: "2px 8px", borderRadius: 4, fontSize: 11, background: "#1a1a1a", color: "#888", textTransform: "capitalize" }}>{m.category}</span>
                      </td>
                      <td style={{ padding: "10px 12px", fontFamily: "'JetBrains Mono', monospace", color: ratio < 1 ? "#f87171" : ratio < 1.2 ? "#fbbf24" : "#e5e5e5" }}>
                        {m.stock.toLocaleString()} {m.unit}
                      </td>
                      <td style={{ padding: "10px 12px", fontFamily: "'JetBrains Mono', monospace", color: "#666" }}>{m.minStock.toLocaleString()} {m.unit}</td>
                      <td style={{ padding: "10px 12px" }}>
                        <div style={{ display: "flex", alignItems: "center", gap: 6 }}>
                          <div style={{ width: 60 }}><ProgressBar value={m.stock} max={m.minStock * 2} color={ratio < 1 ? "#f87171" : ratio < 1.2 ? "#fbbf24" : "#34d399"} /></div>
                          <span style={{ fontSize: 11, color: "#666" }}>{(ratio * 100).toFixed(0)}%</span>
                        </div>
                      </td>
                      <td style={{ padding: "10px 12px", fontFamily: "'JetBrains Mono', monospace", color: "#888" }}>${m.cost.toFixed(2)}</td>
                      <td style={{ padding: "10px 12px", fontFamily: "'JetBrains Mono', monospace", color: "#e5e5e5" }}>${(m.stock * m.cost).toLocaleString("en", { minimumFractionDigits: 2 })}</td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
          <div style={{ marginTop: 16, padding: 16, background: "#0a0a0b", borderRadius: 8, display: "flex", justifyContent: "space-between" }}>
            <span style={{ color: "#888", fontSize: 13 }}>Valor total inventario MP:</span>
            <span style={{ color: "#60a5fa", fontFamily: "'JetBrains Mono', monospace", fontSize: 15, fontWeight: 700 }}>
              ${RAW_MATERIALS.reduce((s, m) => s + m.stock * m.cost, 0).toLocaleString("en", { minimumFractionDigits: 2 })}
            </span>
          </div>
        </>
      )}

      {tab === "finished" && (
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(260px, 1fr))", gap: 16 }}>
          {FINISHED_GOODS.map(g => (
            <Card key={g.id} accent={g.inStock < g.minStock ? "#f87171" : "#34d399"}>
              <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
                <div style={{ color: "#60a5fa33" }}><Icons.Droplet /></div>
                <div style={{ flex: 1 }}>
                  <div style={{ fontSize: 15, fontWeight: 700, color: "#e5e5e5" }}>{g.name}</div>
                  <div style={{ fontSize: 12, color: "#666", marginTop: 2 }}>{g.capacity}L · {g.layers} capas · {g.weight}kg</div>
                </div>
              </div>
              <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 12, marginTop: 16, paddingTop: 16, borderTop: "1px solid #1a1a1a" }}>
                <div>
                  <div style={{ fontSize: 11, color: "#666" }}>En Stock</div>
                  <div style={{ fontSize: 20, fontWeight: 700, color: g.inStock < g.minStock ? "#f87171" : "#e5e5e5", fontFamily: "'JetBrains Mono', monospace" }}>{g.inStock}</div>
                </div>
                <div>
                  <div style={{ fontSize: 11, color: "#666" }}>Reservado</div>
                  <div style={{ fontSize: 20, fontWeight: 700, color: "#fbbf24", fontFamily: "'JetBrains Mono', monospace" }}>{g.reserved}</div>
                </div>
                <div>
                  <div style={{ fontSize: 11, color: "#666" }}>Disponible</div>
                  <div style={{ fontSize: 20, fontWeight: 700, color: "#34d399", fontFamily: "'JetBrains Mono', monospace" }}>{g.inStock - g.reserved}</div>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}

      {tab === "bom" && (
        <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
          {TINACO_MODELS.map(model => (
            <Card key={model.id} title={`${model.name} — BOM`} accent="#c084fc">
              <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
                <thead>
                  <tr style={{ borderBottom: "1px solid #222" }}>
                    {["Material", "Cantidad", "Unidad", "Costo", "Stock Disp.", "Alcance (pzas)"].map(h => (
                      <th key={h} style={{ padding: "8px 12px", textAlign: "left", color: "#666", fontWeight: 600, fontSize: 11, textTransform: "uppercase" }}>{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {(BOM[model.id] || []).map((item, i) => {
                    const mat = RAW_MATERIALS.find(m => m.id === item.materialId);
                    const reach = mat ? Math.floor(mat.stock / item.qty) : 0;
                    return (
                      <tr key={i} style={{ borderBottom: "1px solid #1a1a1a" }}>
                        <td style={{ padding: "8px 12px", color: "#ccc" }}>{mat?.name}</td>
                        <td style={{ padding: "8px 12px", fontFamily: "'JetBrains Mono', monospace", color: "#e5e5e5" }}>{item.qty}</td>
                        <td style={{ padding: "8px 12px", color: "#888" }}>{mat?.unit}</td>
                        <td style={{ padding: "8px 12px", fontFamily: "'JetBrains Mono', monospace", color: "#888" }}>${(item.qty * (mat?.cost || 0)).toFixed(2)}</td>
                        <td style={{ padding: "8px 12px", fontFamily: "'JetBrains Mono', monospace", color: mat && mat.stock <= mat.minStock ? "#f87171" : "#888" }}>{mat?.stock.toLocaleString()}</td>
                        <td style={{ padding: "8px 12px", fontFamily: "'JetBrains Mono', monospace", color: reach < 50 ? "#f87171" : reach < 100 ? "#fbbf24" : "#34d399", fontWeight: 600 }}>{reach}</td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
              <div style={{ marginTop: 12, padding: "8px 12px", background: "#0a0a0b", borderRadius: 6, display: "flex", justifyContent: "space-between" }}>
                <span style={{ color: "#888", fontSize: 12 }}>Costo total materiales/unidad:</span>
                <span style={{ color: "#c084fc", fontFamily: "'JetBrains Mono', monospace", fontWeight: 700 }}>
                  ${(BOM[model.id] || []).reduce((s, item) => {
                    const mat = RAW_MATERIALS.find(m => m.id === item.materialId);
                    return s + item.qty * (mat?.cost || 0);
                  }, 0).toFixed(2)}
                </span>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
};

// ─── Production Module ──────────────────────────────────────────────
const ProductionView = () => {
  const [sortBy, setSortBy] = useState("priority");

  const sorted = [...PRODUCTION_ORDERS].sort((a, b) => {
    if (sortBy === "priority") {
      const p = { alta: 0, media: 1, baja: 2 };
      return (p[a.priority] ?? 2) - (p[b.priority] ?? 2);
    }
    if (sortBy === "status") return a.status.localeCompare(b.status);
    if (sortBy === "progress") return (b.completed / b.qty) - (a.completed / a.qty);
    return 0;
  });

  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
        <div style={{ display: "flex", gap: 8 }}>
          {[["priority", "Prioridad"], ["status", "Estado"], ["progress", "Progreso"]].map(([key, label]) => (
            <button key={key} onClick={() => setSortBy(key)} style={{
              padding: "6px 14px", borderRadius: 6, border: "1px solid",
              borderColor: sortBy === key ? "#c084fc" : "#333",
              background: sortBy === key ? "#3b1a54" : "transparent",
              color: sortBy === key ? "#c084fc" : "#888",
              cursor: "pointer", fontSize: 12, fontWeight: 600,
            }}>Ordenar: {label}</button>
          ))}
        </div>
        <div style={{ fontSize: 12, color: "#666" }}>
          {PRODUCTION_ORDERS.length} órdenes · {PRODUCTION_ORDERS.filter(o => o.status !== "completed").length} activas
        </div>
      </div>

      <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
        {sorted.map(order => (
          <div key={order.id} style={{
            background: "#111214", borderRadius: 10, padding: "16px 20px",
            border: "1px solid #222", display: "grid",
            gridTemplateColumns: "140px 160px 1fr 100px 120px 100px",
            alignItems: "center", gap: 12,
          }}>
            <div>
              <div style={{ fontSize: 14, fontWeight: 700, color: "#e5e5e5", fontFamily: "'JetBrains Mono', monospace" }}>{order.id}</div>
              <div style={{ fontSize: 11, color: "#555", display: "flex", alignItems: "center", gap: 4, marginTop: 2 }}>
                <Icons.Clock /> {order.startDate}
              </div>
            </div>
            <div>
              <div style={{ fontSize: 13, color: "#ccc", fontWeight: 500 }}>{order.modelName}</div>
              {order.machine && <div style={{ fontSize: 11, color: "#555" }}>Máquina: {order.machine}</div>}
            </div>
            <div>
              <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 4 }}>
                <span style={{ fontSize: 12, color: "#888" }}>Progreso</span>
                <span style={{ fontSize: 12, fontFamily: "'JetBrains Mono', monospace", color: "#ccc" }}>
                  {order.completed}/{order.qty} ({((order.completed / order.qty) * 100).toFixed(0)}%)
                </span>
              </div>
              <ProgressBar value={order.completed} max={order.qty} color={order.status === "completed" ? "#34d399" : "#60a5fa"} height={8} />
            </div>
            <StatusBadge status={order.status} />
            <StatusBadge status={order.priority} />
            <div style={{ fontSize: 12, color: "#666", textAlign: "right" }}>
              {order.qty - order.completed} restantes
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

// ─── Assembly Module ────────────────────────────────────────────────
const AssemblyView = () => {
  const [selected, setSelected] = useState(null);
  const passRate = ((ASSEMBLY_LOG.filter(a => !a.defects).length / ASSEMBLY_LOG.length) * 100).toFixed(1);

  return (
    <div>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 16, marginBottom: 24 }}>
        {[
          { label: "Ensamblados Hoy", value: ASSEMBLY_LOG.length, color: "#60a5fa" },
          { label: "Tasa Aprobación", value: `${passRate}%`, color: "#34d399" },
          { label: "Con Defectos", value: ASSEMBLY_LOG.filter(a => a.defects).length, color: "#f87171" },
          { label: "Tiempo Prom.", value: `${(ASSEMBLY_LOG.reduce((s, a) => s + a.duration, 0) / ASSEMBLY_LOG.length).toFixed(1)} min`, color: "#fbbf24" },
        ].map((k, i) => (
          <Card key={i}>
            <div style={{ fontSize: 11, color: "#666", marginBottom: 6 }}>{k.label}</div>
            <div style={{ fontSize: 24, fontWeight: 700, color: k.color, fontFamily: "'JetBrains Mono', monospace" }}>{k.value}</div>
          </Card>
        ))}
      </div>

      <div style={{ display: "grid", gridTemplateColumns: selected ? "1fr 380px" : "1fr", gap: 16 }}>
        <Card title="Registro de Ensamble" accent="#60a5fa">
          <div style={{ maxHeight: 480, overflowY: "auto" }}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
              <thead>
                <tr style={{ borderBottom: "1px solid #222", position: "sticky", top: 0, background: "#111214" }}>
                  {["Serial", "Modelo", "Operador", "Estación", "Tiempo", "Estado", ""].map(h => (
                    <th key={h} style={{ padding: "8px 10px", textAlign: "left", color: "#666", fontWeight: 600, fontSize: 11, textTransform: "uppercase" }}>{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {ASSEMBLY_LOG.map(log => {
                  const allPassed = Object.values(log.steps).every(Boolean) && !log.defects;
                  return (
                    <tr key={log.id} onClick={() => setSelected(log)} style={{
                      borderBottom: "1px solid #1a1a1a", cursor: "pointer",
                      background: selected?.id === log.id ? "#172554" : "transparent",
                    }}>
                      <td style={{ padding: "8px 10px", fontFamily: "'JetBrains Mono', monospace", color: "#ccc", fontSize: 12 }}>{log.tinacoSerial}</td>
                      <td style={{ padding: "8px 10px", color: "#888" }}>{log.modelName}</td>
                      <td style={{ padding: "8px 10px", color: "#888" }}>{log.operator}</td>
                      <td style={{ padding: "8px 10px", color: "#666", fontFamily: "'JetBrains Mono', monospace", fontSize: 12 }}>{log.station}</td>
                      <td style={{ padding: "8px 10px", color: "#888" }}>{log.duration} min</td>
                      <td style={{ padding: "8px 10px" }}>
                        <span style={{ color: allPassed ? "#34d399" : "#f87171" }}>
                          {allPassed ? <Icons.Check /> : <Icons.X />}
                        </span>
                      </td>
                      <td style={{ padding: "8px 10px", fontSize: 11, color: "#f87171" }}>{log.defects || ""}</td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </Card>

        {selected && (
          <Card title={`Detalle: ${selected.tinacoSerial}`} accent={selected.defects ? "#f87171" : "#34d399"}>
            <div style={{ fontSize: 13, color: "#ccc", marginBottom: 16 }}>
              <strong>{selected.modelName}</strong><br />
              <span style={{ color: "#888" }}>Operador: {selected.operator}</span><br />
              <span style={{ color: "#888" }}>Estación: {selected.station}</span><br />
              <span style={{ color: "#888" }}>Hora: {selected.timestamp}</span><br />
              <span style={{ color: "#888" }}>Duración: {selected.duration} min</span>
            </div>

            <h4 style={{ color: "#888", fontSize: 12, fontWeight: 600, marginBottom: 12, textTransform: "uppercase", letterSpacing: 0.5 }}>Checklist de Ensamble</h4>
            <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
              {Object.entries(selected.steps).map(([step, passed]) => {
                const labels = {
                  tapa: "Tapa con cierre", conectores: "Conectores PVC", flotador: "Flotador",
                  base: "Base reforzada", etiqueta: "Etiqueta/Calcomanía", pruebaHermeticidad: "Prueba hermeticidad",
                };
                return (
                  <div key={step} style={{
                    display: "flex", alignItems: "center", gap: 10, padding: "8px 12px",
                    background: passed ? "#0a1a0a" : "#1a0a0a", borderRadius: 6,
                    border: `1px solid ${passed ? "#065f46" : "#7f1d1d"}`,
                  }}>
                    <span style={{ color: passed ? "#34d399" : "#f87171" }}>
                      {passed ? <Icons.Check /> : <Icons.X />}
                    </span>
                    <span style={{ fontSize: 13, color: passed ? "#34d399" : "#f87171" }}>{labels[step] || step}</span>
                  </div>
                );
              })}
            </div>

            {selected.defects && (
              <div style={{ marginTop: 16, padding: 12, background: "#1a0a0a", borderRadius: 8, border: "1px solid #7f1d1d" }}>
                <div style={{ fontSize: 11, color: "#f87171", fontWeight: 600, marginBottom: 4, textTransform: "uppercase" }}>Defecto Detectado</div>
                <div style={{ fontSize: 14, color: "#f87171", fontWeight: 500 }}>{selected.defects}</div>
              </div>
            )}

            <button onClick={() => setSelected(null)} style={{
              marginTop: 16, width: "100%", padding: "8px", borderRadius: 6,
              border: "1px solid #333", background: "#1a1a1a", color: "#888",
              cursor: "pointer", fontSize: 12,
            }}>Cerrar</button>
          </Card>
        )}
      </div>
    </div>
  );
};

// ─── Process Data Module ────────────────────────────────────────────
const ProcessView = () => {
  const [metric, setMetric] = useState("ovenTemp1");

  const metrics = [
    { key: "ovenTemp1", label: "Temp. Horno #1", unit: "°C", color: "#f87171", min: 260, max: 300, target: 282 },
    { key: "ovenTemp2", label: "Temp. Horno #2", unit: "°C", color: "#fb923c", min: 260, max: 300, target: 285 },
    { key: "rpm1", label: "RPM Motor #1", unit: "RPM", color: "#60a5fa", min: 7, max: 11, target: 9 },
    { key: "rpm2", label: "RPM Motor #2", unit: "RPM", color: "#818cf8", min: 7, max: 11, target: 9.2 },
    { key: "coolingTemp", label: "Temp. Enfriamiento", unit: "°C", color: "#22d3ee", min: 30, max: 60, target: 45 },
    { key: "wallThickness", label: "Espesor Pared", unit: "mm", color: "#34d399", min: 4.5, max: 6.0, target: 5.2 },
    { key: "cycleTime", label: "Tiempo Ciclo", unit: "min", color: "#fbbf24", min: 15, max: 28, target: 20 },
  ];

  const current = metrics.find(m => m.key === metric);
  const values = PROCESS_DATA.map(d => d[metric]);
  const avg = (values.reduce((s, v) => s + v, 0) / values.length).toFixed(2);
  const min = Math.min(...values).toFixed(2);
  const max = Math.max(...values).toFixed(2);
  const stdDev = Math.sqrt(values.reduce((s, v) => s + (v - avg) ** 2, 0) / values.length).toFixed(2);

  // Simple SVG chart
  const chartW = 800, chartH = 200, pad = 40;
  const yMin = current.min, yMax = current.max;
  const yRange = yMax - yMin;
  const toX = (i) => pad + (i / (values.length - 1)) * (chartW - pad * 2);
  const toY = (v) => pad + (1 - (v - yMin) / yRange) * (chartH - pad * 2);
  const pathD = values.map((v, i) => `${i === 0 ? "M" : "L"}${toX(i)},${toY(v)}`).join(" ");
  const targetY = toY(current.target);

  return (
    <div>
      <div style={{ display: "flex", gap: 8, marginBottom: 20, flexWrap: "wrap" }}>
        {metrics.map(m => (
          <button key={m.key} onClick={() => setMetric(m.key)} style={{
            padding: "6px 14px", borderRadius: 6, border: `1px solid ${metric === m.key ? m.color : "#333"}`,
            background: metric === m.key ? `${m.color}22` : "transparent",
            color: metric === m.key ? m.color : "#888",
            cursor: "pointer", fontSize: 12, fontWeight: 600, transition: "all 0.2s",
          }}>{m.label}</button>
        ))}
      </div>

      <Card title={`${current.label} — Últimas 24h`} accent={current.color}>
        <svg viewBox={`0 0 ${chartW} ${chartH}`} style={{ width: "100%", height: 220 }}>
          {/* Grid lines */}
          {[0, 0.25, 0.5, 0.75, 1].map(f => {
            const y = pad + f * (chartH - pad * 2);
            const val = (yMax - f * yRange).toFixed(1);
            return (
              <g key={f}>
                <line x1={pad} y1={y} x2={chartW - pad} y2={y} stroke="#222" strokeWidth={1} />
                <text x={pad - 6} y={y + 4} textAnchor="end" fill="#555" fontSize={10}>{val}</text>
              </g>
            );
          })}
          {/* Target line */}
          <line x1={pad} y1={targetY} x2={chartW - pad} y2={targetY} stroke={current.color} strokeWidth={1} strokeDasharray="6 4" opacity={0.5} />
          <text x={chartW - pad + 4} y={targetY + 4} fill={current.color} fontSize={10} opacity={0.7}>Target</text>
          {/* Data line */}
          <path d={pathD} fill="none" stroke={current.color} strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" />
          {/* Area fill */}
          <path d={`${pathD} L${toX(values.length - 1)},${chartH - pad} L${toX(0)},${chartH - pad} Z`} fill={`${current.color}11`} />
          {/* X axis labels */}
          {PROCESS_DATA.filter((_, i) => i % 6 === 0).map((d, i) => (
            <text key={i} x={toX(i * 6)} y={chartH - 8} textAnchor="middle" fill="#555" fontSize={10}>{d.time}</text>
          ))}
        </svg>

        <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 16, marginTop: 20, paddingTop: 16, borderTop: "1px solid #1a1a1a" }}>
          {[
            { label: "Promedio", value: `${avg} ${current.unit}` },
            { label: "Mínimo", value: `${min} ${current.unit}` },
            { label: "Máximo", value: `${max} ${current.unit}` },
            { label: "Desv. Estándar", value: `${stdDev}` },
          ].map((s, i) => (
            <div key={i}>
              <div style={{ fontSize: 11, color: "#666" }}>{s.label}</div>
              <div style={{ fontSize: 16, fontWeight: 700, color: current.color, fontFamily: "'JetBrains Mono', monospace" }}>{s.value}</div>
            </div>
          ))}
        </div>
      </Card>

      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))", gap: 12, marginTop: 16 }}>
        {metrics.map(m => {
          const vals = PROCESS_DATA.map(d => d[m.key]);
          const last = vals[vals.length - 1];
          return (
            <Card key={m.key} style={{ cursor: "pointer", border: metric === m.key ? `1px solid ${m.color}44` : "1px solid #222" }} >
              <div style={{ display: "flex", justifyContent: "space-between", alignItems: "start" }}>
                <div>
                  <div style={{ fontSize: 11, color: "#666" }}>{m.label}</div>
                  <div style={{ fontSize: 20, fontWeight: 700, color: m.color, fontFamily: "'JetBrains Mono', monospace", marginTop: 4 }}>
                    {last.toFixed(1)} <span style={{ fontSize: 12, color: "#555" }}>{m.unit}</span>
                  </div>
                </div>
                <MiniChart data={vals.slice(-20)} color={m.color} />
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
};

// ─── Main App ───────────────────────────────────────────────────────
export default function TinacoProductionSystem() {
  const [activeModule, setActiveModule] = useState("dashboard");
  const [time, setTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => setTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  const modules = [
    { id: "dashboard", label: "Dashboard", icon: <Icons.Dashboard /> },
    { id: "inventory", label: "Inventario", icon: <Icons.Inventory /> },
    { id: "production", label: "Producción", icon: <Icons.Production /> },
    { id: "assembly", label: "Ensamble", icon: <Icons.Assembly /> },
    { id: "process", label: "Proceso", icon: <Icons.Process /> },
  ];

  return (
    <div style={{
      minHeight: "100vh", background: "#09090b", color: "#e5e5e5",
      fontFamily: "'Segoe UI', 'SF Pro Display', -apple-system, sans-serif",
    }}>
      {/* Top bar */}
      <div style={{
        display: "flex", alignItems: "center", justifyContent: "space-between",
        padding: "12px 24px", background: "#0a0a0b", borderBottom: "1px solid #1a1a1a",
        position: "sticky", top: 0, zIndex: 100,
      }}>
        <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            <div style={{ width: 36, height: 36, borderRadius: 8, background: "linear-gradient(135deg, #2563eb, #7c3aed)", display: "flex", alignItems: "center", justifyContent: "center" }}>
              <span style={{ color: "white", fontWeight: 800, fontSize: 14 }}>TP</span>
            </div>
            <div>
              <div style={{ fontSize: 15, fontWeight: 700, color: "#e5e5e5", letterSpacing: -0.3 }}>TinacoPro</div>
              <div style={{ fontSize: 10, color: "#555", letterSpacing: 1, textTransform: "uppercase" }}>Sistema de Producción</div>
            </div>
          </div>

          <div style={{ display: "flex", gap: 4, marginLeft: 24 }}>
            {modules.map(m => (
              <button key={m.id} onClick={() => setActiveModule(m.id)} style={{
                display: "flex", alignItems: "center", gap: 6,
                padding: "8px 16px", borderRadius: 8, border: "none",
                background: activeModule === m.id ? "#1a1a2e" : "transparent",
                color: activeModule === m.id ? "#60a5fa" : "#666",
                cursor: "pointer", fontSize: 13, fontWeight: 500, transition: "all 0.2s",
              }}>
                {m.icon} {m.label}
              </button>
            ))}
          </div>
        </div>

        <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
          <div style={{ display: "flex", alignItems: "center", gap: 6 }}>
            <div style={{ width: 8, height: 8, borderRadius: "50%", background: "#34d399", boxShadow: "0 0 8px #34d39966" }} />
            <span style={{ fontSize: 12, color: "#34d399" }}>En línea</span>
          </div>
          <div style={{ fontSize: 13, color: "#888", fontFamily: "'JetBrains Mono', monospace" }}>
            {time.toLocaleTimeString("es-MX", { hour: "2-digit", minute: "2-digit", second: "2-digit" })}
          </div>
        </div>
      </div>

      {/* Content */}
      <div style={{ padding: "24px", maxWidth: 1400, margin: "0 auto" }}>
        <div style={{ marginBottom: 24 }}>
          <h1 style={{ fontSize: 22, fontWeight: 700, color: "#e5e5e5", margin: 0 }}>
            {modules.find(m => m.id === activeModule)?.label}
          </h1>
          <p style={{ fontSize: 13, color: "#555", margin: "4px 0 0" }}>
            {time.toLocaleDateString("es-MX", { weekday: "long", year: "numeric", month: "long", day: "numeric" })}
          </p>
        </div>

        {activeModule === "dashboard" && <DashboardView />}
        {activeModule === "inventory" && <InventoryView />}
        {activeModule === "production" && <ProductionView />}
        {activeModule === "assembly" && <AssemblyView />}
        {activeModule === "process" && <ProcessView />}
      </div>
    </div>
  );
}
