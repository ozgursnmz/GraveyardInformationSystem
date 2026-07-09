/* ================= Mezarlik Yonetimi - Dashboard mantigi (TR/EN) ================= */

// --- Kimlik dogrulama ---
const token = localStorage.getItem('token');
if (!token) location.href = 'login.html';
const authHeaders = () => ({ 'Authorization': 'Bearer ' + token });

function logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('username');
  localStorage.removeItem('role');
  location.href = 'login.html';
}

// --- Tema (acik/koyu) ---
function applyTheme() {
  const dark = localStorage.getItem('theme') === 'dark';
  document.documentElement.classList.toggle('dark', dark);
  const icon = document.getElementById('themeIcon');
  if (icon) icon.textContent = dark ? 'light_mode' : 'dark_mode';
}

function toggleTheme() {
  const dark = localStorage.getItem('theme') === 'dark';
  localStorage.setItem('theme', dark ? 'light' : 'dark');
  applyTheme();
  if (currentView === 'home') loadCharts(); // grafik renkleri guncellensin
}

// Iki dilli etiket secici (Leaflet'in global L'siyle cakismasin diye 'lbl')
const lbl = (o) => (o && (o[LANG] || o.tr)) || '';

// --- Entity yapilandirmalari ---
// label: i18n anahtari | key: birincil anahtar alan(lar)i | readOnly: sadece okuma
const ENTITIES = {
  gravePlots: {
    label: 'nav_gravePlots', endpoint: 'GravePlots', key: ['plotNumber'],
    columns: [
      { field: 'plotNumber', label: { tr: 'Parsel No', en: 'Plot No' } },
      { field: 'zoneId', label: { tr: 'Bölge', en: 'Zone' } },
      { field: 'status', label: { tr: 'Durum', en: 'Status' }, badge: true },
      { field: 'length', label: { tr: 'Uzunluk', en: 'Length' } },
      { field: 'width', label: { tr: 'Genişlik', en: 'Width' } },
    ],
    fields: [
      { field: 'plotNumber', label: { tr: 'Parsel No', en: 'Plot No' }, type: 'text', required: true, pk: true },
      { field: 'zoneId', label: { tr: 'Bölge ID (ör. Z001)', en: 'Zone ID (e.g. Z001)' }, type: 'text' },
      { field: 'status', label: { tr: 'Durum', en: 'Status' }, type: 'select', options: ['Available', 'Occupied', 'Reserved'] },
      { field: 'length', label: { tr: 'Uzunluk (m)', en: 'Length (m)' }, type: 'number' },
      { field: 'width', label: { tr: 'Genişlik (m)', en: 'Width (m)' }, type: 'number' },
      { field: 'latitude', label: { tr: 'Enlem', en: 'Latitude' }, type: 'number' },
      { field: 'longitude', label: { tr: 'Boylam', en: 'Longitude' }, type: 'number' },
      { field: 'monumentCode', label: { tr: 'Anıt Kodu (ör. M001)', en: 'Monument Code (e.g. M001)' }, type: 'text' },
    ],
  },
  burialRecords: {
    label: 'nav_burialRecords', endpoint: 'BurialRecords', readOnly: true, key: ['ssn'],
    columns: [
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' } },
      { field: 'deceasedName', label: { tr: 'İsim Soyisim', en: 'Full Name' } },
      { field: 'zoneName', label: { tr: 'Bölge', en: 'Zone' } },
      { field: 'plotNumber', label: { tr: 'Parsel', en: 'Plot' } },
      { field: 'dateOfDeath', label: { tr: 'Vefat Tarihi', en: 'Date of Death' } },
      { field: 'religion', label: { tr: 'Din', en: 'Religion' } },
    ],
  },
  graveOwners: {
    label: 'nav_graveOwners', endpoint: 'GraveOwners', key: ['ssn'],
    columns: [
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' } },
      { field: 'ownerType', label: { tr: 'Tür', en: 'Type' }, badge: true },
      { field: 'phoneNumber', label: { tr: 'Telefon', en: 'Phone' } },
      { field: 'email', label: { tr: 'E-posta', en: 'Email' } },
      { field: 'registrationDate', label: { tr: 'Kayıt Tarihi', en: 'Registration Date' } },
    ],
    fields: [
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' }, type: 'text', required: true, pk: true },
      { field: 'ownerType', label: { tr: 'Tür', en: 'Type' }, type: 'select', options: ['Individual', 'Family', 'Institution'] },
      { field: 'phoneNumber', label: { tr: 'Telefon', en: 'Phone' }, type: 'text' },
      { field: 'email', label: { tr: 'E-posta', en: 'Email' }, type: 'text' },
      { field: 'address', label: { tr: 'Adres', en: 'Address' }, type: 'text' },
      { field: 'registrationDate', label: { tr: 'Kayıt Tarihi', en: 'Registration Date' }, type: 'date' },
    ],
  },
  cemeteryZones: {
    label: 'nav_cemeteryZones', endpoint: 'CemeteryZones', key: ['zoneId'],
    columns: [
      { field: 'zoneId', label: { tr: 'Bölge ID', en: 'Zone ID' } },
      { field: 'name', label: { tr: 'Ad', en: 'Name' } },
      { field: 'religionType', label: { tr: 'Din', en: 'Religion' } },
      { field: 'totalCapacity', label: { tr: 'Kapasite', en: 'Capacity' } },
      { field: 'currentOccupancy', label: { tr: 'Dolu', en: 'Occupied' } },
      { field: 'groundType', label: { tr: 'Zemin', en: 'Ground' } },
    ],
    fields: [
      { field: 'zoneId', label: { tr: 'Bölge ID (ör. Z011)', en: 'Zone ID (e.g. Z011)' }, type: 'text', required: true, pk: true },
      { field: 'name', label: { tr: 'Ad', en: 'Name' }, type: 'text' },
      { field: 'religionType', label: { tr: 'Din', en: 'Religion' }, type: 'text' },
      { field: 'totalCapacity', label: { tr: 'Toplam Kapasite', en: 'Total Capacity' }, type: 'number' },
      { field: 'currentOccupancy', label: { tr: 'Mevcut Doluluk', en: 'Current Occupancy' }, type: 'number' },
      { field: 'groundType', label: { tr: 'Zemin Tipi', en: 'Ground Type' }, type: 'text' },
    ],
  },
  payments: {
    label: 'nav_payments', endpoint: 'Payments', key: ['receiptNo'],
    rowAction: { icon: 'receipt_long', fn: 'downloadReceipt' },
    columns: [
      { field: 'receiptNo', label: { tr: 'Makbuz No', en: 'Receipt No' } },
      { field: 'amount', label: { tr: 'Tutar', en: 'Amount' } },
      { field: 'paymentDate', label: { tr: 'Tarih', en: 'Date' } },
      { field: 'paymentMethod', label: { tr: 'Yöntem', en: 'Method' }, badge: true },
      { field: 'ownerSsn', label: { tr: 'Sahip SSN', en: 'Owner SSN' } },
    ],
    fields: [
      { field: 'receiptNo', label: { tr: 'Makbuz No (ör. RCP011)', en: 'Receipt No (e.g. RCP011)' }, type: 'text', required: true, pk: true },
      { field: 'amount', label: { tr: 'Tutar', en: 'Amount' }, type: 'number' },
      { field: 'paymentDate', label: { tr: 'Tarih', en: 'Date' }, type: 'date' },
      { field: 'paymentMethod', label: { tr: 'Yöntem', en: 'Method' }, type: 'select', options: ['Cash', 'Card', 'Transfer'] },
      { field: 'currency', label: { tr: 'Para Birimi', en: 'Currency' }, type: 'text' },
      { field: 'billingAddress', label: { tr: 'Fatura Adresi', en: 'Billing Address' }, type: 'text' },
      { field: 'ownerSsn', label: { tr: 'Sahip SSN', en: 'Owner SSN' }, type: 'text' },
    ],
  },
  maintenanceLogs: {
    label: 'nav_maintenanceLogs', endpoint: 'MaintenanceLogs', key: ['plotNumber', 'logNo'],
    columns: [
      { field: 'plotNumber', label: { tr: 'Parsel No', en: 'Plot No' } },
      { field: 'logNo', label: { tr: 'Kayıt No', en: 'Log No' } },
      { field: 'logDate', label: { tr: 'Tarih', en: 'Date' } },
      { field: 'taskDescription', label: { tr: 'İşlem', en: 'Task' } },
      { field: 'hoursSpent', label: { tr: 'Saat', en: 'Hours' } },
      { field: 'cost', label: { tr: 'Maliyet', en: 'Cost' } },
    ],
    fields: [
      { field: 'plotNumber', label: { tr: 'Parsel No', en: 'Plot No' }, type: 'text', required: true, pk: true },
      { field: 'logNo', label: { tr: 'Kayıt No (ör. LOG002)', en: 'Log No (e.g. LOG002)' }, type: 'text', required: true, pk: true },
      { field: 'logDate', label: { tr: 'Tarih', en: 'Date' }, type: 'date' },
      { field: 'taskDescription', label: { tr: 'İşlem Açıklaması', en: 'Task Description' }, type: 'text' },
      { field: 'hoursSpent', label: { tr: 'Harcanan Saat', en: 'Hours Spent' }, type: 'number' },
      { field: 'cost', label: { tr: 'Maliyet', en: 'Cost' }, type: 'number' },
      { field: 'employeeId', label: { tr: 'Çalışan ID (ör. EMP004)', en: 'Employee ID (e.g. EMP004)' }, type: 'text' },
    ],
  },
};

let currentKey = 'gravePlots';
let currentView = 'home';
let currentData = [];
let currentMonths = 0; // 0 = tum zamanlar

const el = (id) => document.getElementById(id);
const fmt = (v) => (v === null || v === undefined || v === '' ? '—' : v);
const keyPath = (cfg, item) => cfg.key.map((k) => item[k]).join('/');

function badgeHtml(value) {
  const v = (value || '').toString();
  let cls = 'bg-surface-container-high text-secondary border-outline-variant';
  if (v === 'Occupied') cls = 'bg-[#f1f5f9] text-[#475569] border-outline-variant';
  else if (v === 'Reserved') cls = 'bg-primary-fixed-dim text-on-primary-fixed border-primary-fixed';
  return `<span class="px-3 py-1 rounded-full text-xs font-semibold border ${cls}">${fmt(v)}</span>`;
}

// --- Istatistik kartlari ---
async function loadStats() {
  try {
    const res = await fetch('/api/Stats?months=' + currentMonths);
    const s = await res.json();
    const loc = LANG === 'tr' ? 'tr-TR' : 'en-US';
    el('statTotalPlots').textContent = s.totalPlots.toLocaleString(loc);
    el('statOccupancy').textContent = '%' + s.occupancyRate;
    el('statOccupancyBar').style.width = s.occupancyRate + '%';
    el('statDeceased').textContent = s.totalDeceased.toLocaleString(loc);
    el('statRevenue').textContent = s.totalRevenue.toLocaleString(loc) + ' ₺';
    el('statExpense').textContent = s.totalExpense.toLocaleString(loc) + ' ₺';
    const net = el('statNet');
    net.textContent = s.netProfit.toLocaleString(loc) + ' ₺';
    net.classList.toggle('text-error', s.netProfit < 0);
  } catch (e) { /* sessiz */ }
}

// --- Grafikler (Chart.js) ---
let charts = {};

function drawChart(canvasId, type, data) {
  const ctx = document.getElementById(canvasId);
  if (!ctx || typeof Chart === 'undefined') return;
  if (charts[canvasId]) charts[canvasId].destroy();
  const dark = document.documentElement.classList.contains('dark');
  const txt = dark ? '#c4c8bc' : '#4a4e4a';
  const grid = dark ? 'rgba(255,255,255,.08)' : 'rgba(0,0,0,.08)';
  charts[canvasId] = new Chart(ctx, {
    type, data,
    options: {
      responsive: true,
      plugins: { legend: { display: type === 'doughnut', position: 'bottom', labels: { color: txt } } },
      scales: type === 'doughnut' ? {} : {
        y: { beginAtZero: true, ticks: { precision: 0, color: txt }, grid: { color: grid } },
        x: { ticks: { color: txt }, grid: { color: grid } },
      },
    },
  });
}

async function loadCharts() {
  if (typeof Chart === 'undefined') return;
  try {
    const res = await fetch('/api/Stats/charts?months=' + currentMonths);
    const d = await res.json();
    const dark = document.documentElement.classList.contains('dark');
    const green = dark ? '#6bfb9a' : '#4a7c59';
    const greenDim = dark ? '#4de082' : '#8ecf9e';
    const slate = '#8aa0c0', gold = dark ? '#dcc48e' : '#705c30', mid = '#c4a66a';
    const loc = LANG === 'tr' ? 'tr-TR' : 'en-US';

    drawChart('chartZone', 'bar', {
      labels: d.zoneOccupancy.map((x) => x.label),
      datasets: [{ data: d.zoneOccupancy.map((x) => x.value), backgroundColor: green, borderRadius: 6 }],
    });

    const monthLabels = d.deathsByMonth.map((x) =>
      new Date(2024, parseInt(x.label) - 1, 1).toLocaleDateString(loc, { month: 'short' }));
    drawChart('chartDeaths', 'line', {
      labels: monthLabels,
      datasets: [{ data: d.deathsByMonth.map((x) => x.value), borderColor: green, backgroundColor: greenDim, fill: true, tension: 0.3 }],
    });

    drawChart('chartPayments', 'doughnut', {
      labels: d.paymentMethods.map((x) => x.label),
      datasets: [{ data: d.paymentMethods.map((x) => x.value), backgroundColor: [green, gold, slate, greenDim, mid] }],
    });

    // Gelir / Gider / Net - bar
    const net = d.income - d.expense;
    drawChart('chartFinance', 'bar', {
      labels: [t('fin_income'), t('fin_expense'), t('fin_net')],
      datasets: [{ data: [d.income, d.expense, net], backgroundColor: [green, '#b83230', slate], borderRadius: 6 }],
    });

    // Aylara gore bakim maliyeti - line
    const maintMonths = d.maintenanceByMonth.map((x) =>
      new Date(2024, parseInt(x.label) - 1, 1).toLocaleDateString(loc, { month: 'short' }));
    drawChart('chartMaintenance', 'line', {
      labels: maintMonths,
      datasets: [{ data: d.maintenanceByMonth.map((x) => x.value), borderColor: gold, backgroundColor: 'rgba(112,92,48,.15)', fill: true, tension: 0.3 }],
    });
  } catch (e) { /* sessiz */ }
}

// --- Gorunum: sidebar aktiflik ---
function highlightNav(id) {
  document.querySelectorAll('nav a[data-entity], nav a[data-view]').forEach((a) => {
    const key = a.dataset.entity || a.dataset.view;
    const active = key === id;
    a.classList.toggle('bg-primary-container', active);
    a.classList.toggle('text-on-primary-container', active);
    a.classList.toggle('font-bold', active);
    a.classList.toggle('text-secondary', !active);
  });
}

// --- Donem filtresi ---
function highlightPeriod() {
  document.querySelectorAll('#periodBar button').forEach((b) => {
    const active = parseInt(b.dataset.period) === currentMonths;
    b.classList.toggle('bg-primary', active);
    b.classList.toggle('text-on-primary', active);
    b.classList.toggle('border-primary', active);
  });
}

function setPeriod(months) {
  currentMonths = months;
  highlightPeriod();
  loadStats();
  loadCharts();
}

// --- Ana sayfa gorunumu (istatistik + grafik) ---
function showHome() {
  currentView = 'home';
  el('homeView').classList.remove('hidden');
  el('tableView').classList.add('hidden');
  el('mapView').classList.add('hidden');
  el('calendarView').classList.add('hidden');
  el('btnAdd').style.display = 'none';
  el('pageTitle').textContent = t('nav_home');
  highlightNav('home');
  highlightPeriod();
  loadStats();
  loadCharts();
}

// --- Harita gorunumu (Leaflet) ---
let leafletMap = null;
let mapMarkers = null;

function statusColor(s) {
  if (s === 'Occupied') return '#6b6358';
  if (s === 'Available') return '#4a7c59';
  if (s === 'Reserved') return '#705c30';
  return '#9ca3af';
}
function statusLabel(s) {
  const k = 'st_' + s;
  return t(k) === k ? (s || '—') : t(k);
}

function showMap() {
  currentView = 'map';
  el('homeView').classList.add('hidden');
  el('tableView').classList.add('hidden');
  el('calendarView').classList.add('hidden');
  el('mapView').classList.remove('hidden');
  el('btnAdd').style.display = 'none';
  el('pageTitle').textContent = t('nav_map');
  highlightNav('map');

  if (typeof L === 'undefined') return;
  if (!leafletMap) {
    leafletMap = L.map('map').setView([37.05, 35.35], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap', maxZoom: 19,
    }).addTo(leafletMap);
    mapMarkers = L.layerGroup().addTo(leafletMap);
  }
  // Kutu yeni gorunur oldugu icin boyutu birkac kez tazele (gri kalmasin)
  requestAnimationFrame(() => leafletMap.invalidateSize());
  setTimeout(() => leafletMap.invalidateSize(), 200);
  setTimeout(() => leafletMap.invalidateSize(), 600);
  loadMapMarkers();
}

async function loadMapMarkers() {
  if (!mapMarkers) return;
  try {
    const res = await fetch('/api/GravePlots/map');
    const plots = await res.json();
    mapMarkers.clearLayers();
    const pts = [];
    plots.forEach((p) => {
      if (p.latitude == null || p.longitude == null) return;
      const m = L.circleMarker([p.latitude, p.longitude], {
        radius: 9, color: '#fff', weight: 2, fillColor: statusColor(p.status), fillOpacity: 0.9,
      });
      m.bindPopup(
        `<b>${p.plotNumber}</b><br>${t('map_zone')}: ${p.zoneName || '—'}<br>` +
        `${statusLabel(p.status)}<br>${t('map_occupant')}: ${p.occupant || t('map_empty')}`
      );
      m.addTo(mapMarkers);
      pts.push([p.latitude, p.longitude]);
    });
    if (pts.length) leafletMap.fitBounds(pts, { padding: [40, 40], maxZoom: 15 });
    leafletMap.invalidateSize();
  } catch (e) { /* sessiz */ }
}

// --- Cenaze takvimi (FullCalendar) ---
let calendarObj = null;

async function fetchCalendarEvents() {
  try {
    const res = await fetch('/api/FuneralServices/calendar');
    const data = await res.json();
    return data
      .filter((e) => e.serviceDate)
      .map((e) => ({
        title: (e.deceasedName || e.serviceType || 'Servis'),
        start: e.startTime ? `${e.serviceDate}T${e.startTime}` : e.serviceDate,
        end: e.endTime ? `${e.serviceDate}T${e.endTime}` : undefined,
        extendedProps: { serviceType: e.serviceType, attendees: e.expectedAttendees },
      }));
  } catch (e) { return []; }
}

async function showCalendar() {
  currentView = 'calendar';
  el('homeView').classList.add('hidden');
  el('tableView').classList.add('hidden');
  el('mapView').classList.add('hidden');
  el('calendarView').classList.remove('hidden');
  el('btnAdd').style.display = 'none';
  el('pageTitle').textContent = t('nav_calendar');
  highlightNav('calendar');

  if (typeof FullCalendar === 'undefined') return;
  const events = await fetchCalendarEvents();

  if (!calendarObj) {
    calendarObj = new FullCalendar.Calendar(el('calendar'), {
      initialView: 'dayGridMonth',
      locale: LANG,
      initialDate: events.length ? events[0].start.slice(0, 10) : undefined,
      headerToolbar: { left: 'prev,next today', center: 'title', right: 'dayGridMonth,listMonth' },
      height: 640,
      events,
      eventClick: (info) => {
        const p = info.event.extendedProps;
        alert(`${info.event.title}\n${t('cal_service')}: ${p.serviceType || '—'}\n${t('cal_attendees')}: ${p.attendees ?? '—'}`);
      },
    });
    calendarObj.render();
  } else {
    calendarObj.setOption('locale', LANG);
    calendarObj.removeAllEvents();
    calendarObj.addEventSource(events);
    calendarObj.updateSize();
  }
}

// --- Tablo gorunumu ---
async function loadEntity(entityKey) {
  currentKey = entityKey;
  currentView = entityKey;
  const cfg = ENTITIES[entityKey];

  el('homeView').classList.add('hidden');
  el('mapView').classList.add('hidden');
  el('calendarView').classList.add('hidden');
  el('tableView').classList.remove('hidden');
  highlightNav(entityKey);

  el('pageTitle').textContent = t(cfg.label);
  el('tableTitle').textContent = t(cfg.label);
  el('btnAdd').style.display = cfg.readOnly ? 'none' : 'flex';

  el('tableHead').innerHTML =
    cfg.columns.map((c) => `<th class="py-4 px-6 font-semibold">${lbl(c.label)}</th>`).join('') +
    (cfg.readOnly ? '' : `<th class="py-4 px-6 font-semibold text-right">${t('actions')}</th>`);

  el('tableBody').innerHTML = `<tr><td class="py-8 px-6 text-center text-secondary" colspan="99">${t('loading')}</td></tr>`;

  try {
    const res = await fetch('/api/' + cfg.endpoint, { headers: authHeaders() });
    currentData = await res.json();
    renderRows();
  } catch (e) {
    el('tableBody').innerHTML = `<tr><td class="py-8 px-6 text-center text-error" colspan="99">${t('load_error')}</td></tr>`;
  }
}

function renderRows() {
  const cfg = ENTITIES[currentKey];
  const q = el('searchInput').value.trim().toLowerCase();
  let rows = currentData;
  if (q) {
    rows = rows.filter((item) =>
      cfg.columns.some((c) => (item[c.field] ?? '').toString().toLowerCase().includes(q))
    );
  }

  if (!rows.length) {
    el('tableBody').innerHTML = `<tr><td class="py-8 px-6 text-center text-secondary" colspan="99">${t('no_records')}</td></tr>`;
    el('pageInfo').textContent = t('total') + ': 0';
    return;
  }

  el('tableBody').innerHTML = rows.map((item) => {
    const cells = cfg.columns.map((c) => {
      const val = item[c.field];
      if (c.badge) return `<td class="py-4 px-6">${badgeHtml(val)}</td>`;
      return `<td class="py-4 px-6">${fmt(val)}</td>`;
    }).join('');

    const kp = JSON.stringify(keyPath(cfg, item));
    const extra = cfg.rowAction
      ? `<button onclick='${cfg.rowAction.fn}(${kp})' class="p-2 text-secondary hover:text-primary rounded-full hover:bg-surface-container-high"><span class="material-symbols-outlined text-[20px]">${cfg.rowAction.icon}</span></button>`
      : '';
    const actions = cfg.readOnly ? '' : `
      <td class="py-4 px-6 text-right">
        <div class="flex justify-end gap-2">
          ${extra}
          <button onclick='openEdit(${kp})' class="p-2 text-secondary hover:text-primary rounded-full hover:bg-surface-container-high"><span class="material-symbols-outlined text-[20px]">edit</span></button>
          <button onclick='deleteRow(${kp})' class="p-2 text-secondary hover:text-error rounded-full hover:bg-error-container"><span class="material-symbols-outlined text-[20px]">delete</span></button>
        </div>
      </td>`;

    return `<tr class="hover:bg-surface-container-low transition-colors">${cells}${actions}</tr>`;
  }).join('');

  el('pageInfo').textContent = `${t('shown')}: ${rows.length} / ${t('total')}: ${currentData.length}`;
}

// --- Modal / Form ---
function buildForm(cfg, item) {
  return cfg.fields.map((f) => {
    const val = item ? (item[f.field] ?? '') : '';
    const disabled = item && f.pk ? 'disabled' : '';
    let input;
    if (f.type === 'select') {
      input = `<select id="f_${f.field}" ${disabled} class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg focus:outline-none focus:ring-2 focus:ring-primary appearance-none cursor-pointer disabled:opacity-60">
        <option value="">${t('select')}</option>
        ${f.options.map((o) => `<option value="${o}" ${o === val ? 'selected' : ''}>${o}</option>`).join('')}
      </select>`;
    } else {
      input = `<input id="f_${f.field}" type="${f.type}" value="${val}" ${f.required ? 'required' : ''} ${disabled}
        class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60" step="any"/>`;
    }
    return `<div class="flex flex-col gap-2">
      <label class="text-xs font-semibold tracking-wide text-on-surface" for="f_${f.field}">${lbl(f.label)}</label>
      ${input}
    </div>`;
  }).join('');
}

let editingId = null;

function openAdd() {
  const cfg = ENTITIES[currentKey];
  if (cfg.readOnly) return;
  editingId = null;
  el('modalTitle').textContent = t('modal_add');
  el('formFields').innerHTML = buildForm(cfg, null);
  el('modalError').classList.add('hidden');
  el('modal').classList.remove('hidden');
}

function openEdit(id) {
  const cfg = ENTITIES[currentKey];
  const item = currentData.find((x) => keyPath(cfg, x) === id);
  if (!item) return;
  editingId = id;
  el('modalTitle').textContent = t('modal_edit');
  el('formFields').innerHTML = buildForm(cfg, item);
  el('modalError').classList.add('hidden');
  el('modal').classList.remove('hidden');
}

function closeModal() { el('modal').classList.add('hidden'); }

async function saveRecord() {
  const cfg = ENTITIES[currentKey];
  const body = {};
  cfg.fields.forEach((f) => {
    let v = el('f_' + f.field).value;
    if (v === '') { body[f.field] = null; return; }
    body[f.field] = f.type === 'number' ? parseFloat(v) : v;
  });

  const isEdit = editingId !== null;
  const url = '/api/' + cfg.endpoint + (isEdit ? '/' + editingId : '');
  const method = isEdit ? 'PUT' : 'POST';

  try {
    const res = await fetch(url, {
      method,
      headers: { ...authHeaders(), 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    });
    if (res.status === 401 || res.status === 403) { showModalError(t('err_auth')); return; }
    if (!res.ok) { showModalError(friendlyError(await res.text(), res.status)); return; }
    closeModal();
    await loadEntity(currentKey);
    await loadStats();
  } catch (e) {
    showModalError(t('err_generic'));
  }
}

async function deleteRow(id) {
  const cfg = ENTITIES[currentKey];
  if (!confirm(t('confirm_delete'))) return;
  try {
    const res = await fetch('/api/' + cfg.endpoint + '/' + id, { method: 'DELETE', headers: authHeaders() });
    if (res.status === 401 || res.status === 403) { alert(t('err_auth')); return; }
    if (!res.ok) {
      const txt = await res.text();
      alert(txt.includes('REFERENCE') || txt.includes('FOREIGN KEY') ? t('delete_fk') : t('err_generic'));
      return;
    }
    await loadEntity(currentKey);
    await loadStats();
  } catch (e) { alert(t('err_generic')); }
}

// Aktif tabloyu (arama filtresi uygulanmis) Excel'e aktar
function exportExcel() {
  const cfg = ENTITIES[currentKey];
  if (typeof XLSX === 'undefined') return;

  const q = el('searchInput').value.trim().toLowerCase();
  let data = currentData;
  if (q) data = data.filter((item) =>
    cfg.columns.some((c) => (item[c.field] ?? '').toString().toLowerCase().includes(q)));

  const rows = data.map((item) => {
    const o = {};
    cfg.columns.forEach((c) => { o[lbl(c.label)] = item[c.field] ?? ''; });
    return o;
  });

  const ws = XLSX.utils.json_to_sheet(rows);
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, t(cfg.label).slice(0, 31));
  XLSX.writeFile(wb, cfg.endpoint + '.xlsx');
}

// jsPDF standart fontu Turkce ozel karakterleri desteklemez -> ASCII'ye cevir
function pdfSafe(s) {
  return (s == null ? '' : String(s))
    .replace(/ş/g, 's').replace(/Ş/g, 'S').replace(/ğ/g, 'g').replace(/Ğ/g, 'G')
    .replace(/ı/g, 'i').replace(/İ/g, 'I').replace(/ç/g, 'c').replace(/Ç/g, 'C')
    .replace(/ö/g, 'o').replace(/Ö/g, 'O').replace(/ü/g, 'u').replace(/Ü/g, 'U');
}

// Odeme makbuzu PDF olustur
function downloadReceipt(id) {
  const p = currentData.find((x) => x.receiptNo === id);
  if (!p || !window.jspdf) return;
  const tr = LANG === 'tr';
  const doc = new window.jspdf.jsPDF();

  doc.setFontSize(16);
  doc.text(pdfSafe(tr ? 'Mezarlik Yonetim Sistemi' : 'Cemetery Management System'), 20, 22);
  doc.setFontSize(12);
  doc.text(pdfSafe(tr ? 'Odeme Makbuzu' : 'Payment Receipt'), 20, 32);
  doc.setLineWidth(0.3);
  doc.line(20, 36, 190, 36);

  const rows = [
    [tr ? 'Makbuz No' : 'Receipt No', p.receiptNo],
    [tr ? 'Tutar' : 'Amount', (p.amount ?? '') + ' ' + (p.currency || '')],
    [tr ? 'Tarih' : 'Date', p.paymentDate || ''],
    [tr ? 'Odeme Yontemi' : 'Method', p.paymentMethod || ''],
    [tr ? 'Sahip SSN' : 'Owner SSN', p.ownerSsn || ''],
    [tr ? 'Fatura Adresi' : 'Billing Address', p.billingAddress || ''],
  ];

  let y = 50;
  doc.setFontSize(11);
  rows.forEach(([k, v]) => {
    doc.setFont(undefined, 'bold');
    doc.text(pdfSafe(k) + ':', 20, y);
    doc.setFont(undefined, 'normal');
    doc.text(pdfSafe(v), 75, y);
    y += 11;
  });

  doc.setFontSize(9);
  doc.text(pdfSafe((tr ? 'Olusturulma: ' : 'Generated: ') +
    new Date().toLocaleString(tr ? 'tr-TR' : 'en-US')), 20, y + 12);

  doc.save('makbuz_' + p.receiptNo + '.pdf');
}

function showModalError(msg) {
  const box = el('modalError');
  box.textContent = msg;
  box.classList.remove('hidden');
}

function friendlyError(text, status) {
  const s = (text || '').toString();
  if (s.includes('PRIMARY KEY') || s.includes('duplicate key')) return t('err_duplicate');
  if (s.includes('FOREIGN KEY') || s.includes('REFERENCE')) return t('err_fk');
  if (s.includes('CHECK constraint')) return t('err_check');
  if (s.includes('NULL')) return t('err_null');
  return t('err_generic');
}

// Dil degisince aktif gorunumu yenile
window.onLangChange = () => {
  if (currentView === 'home') showHome();
  else if (currentView === 'map') showMap();
  else if (currentView === 'calendar') showCalendar();
  else loadEntity(currentView);
};

// --- Baslat ---
document.addEventListener('DOMContentLoaded', () => {
  applyTheme();
  applyI18n();
  el('username').textContent = localStorage.getItem('username') || 'admin';
  document.querySelectorAll('nav a[data-entity]').forEach((a) => {
    a.addEventListener('click', (e) => { e.preventDefault(); loadEntity(a.dataset.entity); });
  });
  document.querySelectorAll('nav a[data-view]').forEach((a) => {
    a.addEventListener('click', (e) => {
      e.preventDefault();
      if (a.dataset.view === 'map') showMap();
      else if (a.dataset.view === 'calendar') showCalendar();
      else showHome();
    });
  });
  el('searchInput').addEventListener('input', renderRows);
  el('btnAdd').addEventListener('click', openAdd);
  el('btnSave').addEventListener('click', saveRecord);
  el('btnCancel').addEventListener('click', closeModal);
  el('btnClose').addEventListener('click', closeModal);
  el('btnLogout').addEventListener('click', logout);

  showHome();
});
