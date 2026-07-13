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

// --- Mobil sidebar ---
function openSidebar() {
  const s = document.getElementById('sidebar');
  s.classList.remove('-translate-x-full');
  s.classList.add('translate-x-0');
  document.getElementById('sidebarOverlay').classList.remove('hidden');
}
function closeSidebar() {
  const s = document.getElementById('sidebar');
  s.classList.add('-translate-x-full');
  s.classList.remove('translate-x-0');
  document.getElementById('sidebarOverlay').classList.add('hidden');
}

// --- Kullanici menusu (sag ust) ---
function toggleUserMenu() {
  const m = document.getElementById('userMenu');
  if (m) m.classList.toggle('hidden');
}
document.addEventListener('click', (e) => {
  const b = document.getElementById('userBtn');
  const m = document.getElementById('userMenu');
  if (m && b && !b.contains(e.target) && !m.contains(e.target)) m.classList.add('hidden');
});

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
  people: {
    label: 'nav_people', endpoint: 'People', key: ['ssn'],
    columns: [
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' } },
      { field: 'firstName', label: { tr: 'Ad', en: 'First Name' } },
      { field: 'lastName', label: { tr: 'Soyad', en: 'Last Name' } },
      { field: 'motherName', label: { tr: 'Anne Adı', en: 'Mother' } },
      { field: 'fatherName', label: { tr: 'Baba Adı', en: 'Father' } },
      { field: 'dateOfBirth', label: { tr: 'Doğum Tarihi', en: 'Date of Birth' } },
      { field: 'gender', label: { tr: 'Cinsiyet', en: 'Gender' } },
    ],
    fields: [
      { field: 'ssn', label: { tr: 'TC / SSN (11 hane)', en: 'SSN (11 digits)' }, type: 'text', required: true, pk: true },
      { field: 'firstName', label: { tr: 'Ad', en: 'First Name' }, type: 'text' },
      { field: 'lastName', label: { tr: 'Soyad', en: 'Last Name' }, type: 'text' },
      { field: 'motherName', label: { tr: 'Anne Adı', en: 'Mother’s Name' }, type: 'text' },
      { field: 'fatherName', label: { tr: 'Baba Adı', en: 'Father’s Name' }, type: 'text' },
      { field: 'dateOfBirth', label: { tr: 'Doğum Tarihi', en: 'Date of Birth' }, type: 'date' },
      { field: 'gender', label: { tr: 'Cinsiyet', en: 'Gender' }, type: 'select', options: ['Male', 'Female', 'Unknown'] },
    ],
  },
  gravePlots: {
    label: 'nav_gravePlots', endpoint: 'GravePlots', key: ['plotNumber'],
    columns: [
      { field: 'plotNumber', label: { tr: 'Parsel No', en: 'Plot No' } },
      { field: 'zoneId', label: { tr: 'Bölge', en: 'Zone' }, filter: true },
      { field: 'status', label: { tr: 'Durum', en: 'Status' }, badge: true, filter: true },
      { field: 'length', label: { tr: 'Uzunluk', en: 'Length' } },
      { field: 'width', label: { tr: 'Genişlik', en: 'Width' } },
    ],
    fields: [
      { field: 'plotNumber', label: { tr: 'Parsel No', en: 'Plot No' }, type: 'text', required: true, pk: true },
      { field: 'zoneId', label: { tr: 'Bölge', en: 'Zone' }, type: 'refSelect', ref: { endpoint: 'CemeteryZones', value: 'zoneId', label: 'name' } },
      { field: 'status', label: { tr: 'Durum', en: 'Status' }, type: 'select', options: ['Available', 'Occupied', 'Reserved', 'Maintenance'] },
      { field: 'length', label: { tr: 'Uzunluk (m)', en: 'Length (m)' }, type: 'number' },
      { field: 'width', label: { tr: 'Genişlik (m)', en: 'Width (m)' }, type: 'number' },
      { field: 'latitude', label: { tr: 'Enlem', en: 'Latitude' }, type: 'number' },
      { field: 'longitude', label: { tr: 'Boylam', en: 'Longitude' }, type: 'number' },
      { field: 'monumentCode', label: { tr: 'Anıt', en: 'Monument' }, type: 'refSelect', ref: { endpoint: 'MonumentTypes', value: 'monumentCode', labelFields: ['material', 'style'] } },
    ],
  },
  burialRecords: {
    label: 'nav_burialRecords', endpoint: 'BurialRecords', writeEndpoint: 'DeceasedPeople', key: ['ssn'],
    columns: [
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' } },
      { field: 'deceasedName', label: { tr: 'İsim Soyisim', en: 'Full Name' } },
      { field: 'zoneName', label: { tr: 'Bölge', en: 'Zone' }, filter: true },
      { field: 'plotNumber', label: { tr: 'Parsel', en: 'Plot' } },
      { field: 'dateOfDeath', label: { tr: 'Vefat Tarihi', en: 'Date of Death' } },
      { field: 'religion', label: { tr: 'Din', en: 'Religion' }, filter: true },
    ],
    writeEndpoint: 'DeceasedPeople/full',
    fields: [
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' }, type: 'text', required: true, pk: true },
      { field: 'firstName', label: { tr: 'Ad', en: 'First Name' }, type: 'text' },
      { field: 'lastName', label: { tr: 'Soyad', en: 'Last Name' }, type: 'text' },
      { field: 'dateOfBirth', label: { tr: 'Doğum Tarihi', en: 'Date of Birth' }, type: 'date' },
      { field: 'gender', label: { tr: 'Cinsiyet', en: 'Gender' }, type: 'select', options: ['Male', 'Female', 'Unknown'] },
      { field: 'motherName', label: { tr: 'Anne Adı', en: 'Mother’s Name' }, type: 'text' },
      { field: 'fatherName', label: { tr: 'Baba Adı', en: 'Father’s Name' }, type: 'text' },
      { field: 'dateOfDeath', label: { tr: 'Vefat Tarihi', en: 'Date of Death' }, type: 'date' },
      { field: 'burialDate', label: { tr: 'Defin Tarihi', en: 'Burial Date' }, type: 'date' },
      { field: 'causeOfDeath', label: { tr: 'Ölüm Nedeni', en: 'Cause of Death' }, type: 'text' },
      { field: 'religion', label: { tr: 'Din', en: 'Religion' }, type: 'text' },
      { field: 'veteranStatus', label: { tr: 'Gazi/Şehit', en: 'Veteran' }, type: 'select', options: ['Yes', 'No'] },
      { field: 'funeralPreferences', label: { tr: 'Cenaze Tercihleri', en: 'Funeral Preferences' }, type: 'text' },
      { field: 'plotNumber', label: { tr: 'Parsel (boş bırakılabilir)', en: 'Plot (optional)' }, type: 'plotSelect' },
      { field: 'permitNumber', label: { tr: 'Defin İzni (boş bırakılabilir)', en: 'Permit (optional)' }, type: 'permitSelect' },
    ],
  },
  graveOwners: {
    label: 'nav_graveOwners', endpoint: 'GraveOwners', key: ['ssn'],
    columns: [
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' } },
      { field: 'ownerType', label: { tr: 'Tür', en: 'Type' }, badge: true, filter: true },
      { field: 'relationship', label: { tr: 'Yakınlık', en: 'Relationship' } },
      { field: 'phoneNumber', label: { tr: 'Telefon', en: 'Phone' } },
      { field: 'email', label: { tr: 'E-posta', en: 'Email' } },
      { field: 'registrationDate', label: { tr: 'Kayıt Tarihi', en: 'Registration Date' } },
    ],
    fields: [
      { field: 'ssn', label: { tr: 'Kişi (TC / SSN)', en: 'Person (SSN)' }, type: 'refSelect', required: true, pk: true, ref: { endpoint: 'People', value: 'ssn', labelFields: ['firstName', 'lastName'] } },
      { field: 'ownerType', label: { tr: 'Tür', en: 'Type' }, type: 'select', options: ['Individual', 'Family', 'Institution'] },
      { field: 'relationship', label: { tr: 'Meftuna Yakınlık Derecesi', en: 'Relationship to Deceased' }, type: 'text' },
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
      { field: 'totalCapacity', label: { tr: 'Kapasite', en: 'Capacity' } },
      { field: 'currentOccupancy', label: { tr: 'Dolu', en: 'Occupied' } },
      { field: 'groundType', label: { tr: 'Zemin', en: 'Ground' } },
    ],
    fields: [
      { field: 'zoneId', label: { tr: 'Bölge ID (ör. Z011)', en: 'Zone ID (e.g. Z011)' }, type: 'text', required: true, pk: true },
      { field: 'name', label: { tr: 'Ad', en: 'Name' }, type: 'text' },
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
      { field: 'paymentMethod', label: { tr: 'Yöntem', en: 'Method' }, badge: true, filter: true },
      { field: 'ownerSsn', label: { tr: 'Sahip SSN', en: 'Owner SSN' } },
    ],
    fields: [
      { field: 'receiptNo', label: { tr: 'Makbuz No (ör. RCP011)', en: 'Receipt No (e.g. RCP011)' }, type: 'text', required: true, pk: true },
      { field: 'amount', label: { tr: 'Tutar', en: 'Amount' }, type: 'number' },
      { field: 'paymentDate', label: { tr: 'Tarih', en: 'Date' }, type: 'date' },
      { field: 'paymentMethod', label: { tr: 'Yöntem', en: 'Method' }, type: 'select', options: ['Cash', 'Card', 'Transfer'] },
      { field: 'currency', label: { tr: 'Para Birimi', en: 'Currency' }, type: 'text' },
      { field: 'billingAddress', label: { tr: 'Fatura Adresi', en: 'Billing Address' }, type: 'text' },
      { field: 'ownerSsn', label: { tr: 'Sahip', en: 'Owner' }, type: 'refSelect', ref: { endpoint: 'GraveOwners', value: 'ssn', label: 'ownerType' } },
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
      { field: 'plotNumber', label: { tr: 'Parsel', en: 'Plot' }, type: 'refSelect', required: true, pk: true, ref: { endpoint: 'GravePlots', value: 'plotNumber', label: 'status' } },
      { field: 'logNo', label: { tr: 'Kayıt No (ör. LOG002)', en: 'Log No (e.g. LOG002)' }, type: 'text', required: true, pk: true },
      { field: 'logDate', label: { tr: 'Tarih', en: 'Date' }, type: 'date' },
      { field: 'taskDescription', label: { tr: 'İşlem Açıklaması', en: 'Task Description' }, type: 'text' },
      { field: 'hoursSpent', label: { tr: 'Harcanan Saat', en: 'Hours Spent' }, type: 'number' },
      { field: 'cost', label: { tr: 'Maliyet', en: 'Cost' }, type: 'number' },
      { field: 'employeeId', label: { tr: 'Çalışan', en: 'Employee' }, type: 'refSelect', ref: { endpoint: 'Employees', value: 'employeeId', label: 'jobTitle' } },
    ],
  },
  employees: {
    label: 'nav_employees', endpoint: 'Employees', key: ['employeeId'],
    columns: [
      { field: 'employeeId', label: { tr: 'ID', en: 'ID' } },
      { field: 'ssn', label: { tr: 'TC / SSN', en: 'SSN' } },
      { field: 'jobTitle', label: { tr: 'Görev', en: 'Job Title' } },
      { field: 'shift', label: { tr: 'Vardiya', en: 'Shift' }, badge: true, filter: true },
      { field: 'salary', label: { tr: 'Maaş', en: 'Salary' } },
      { field: 'hireDate', label: { tr: 'İşe Giriş', en: 'Hire Date' } },
    ],
    fields: [
      { field: 'employeeId', label: { tr: 'Çalışan ID (ör. EMP011)', en: 'Employee ID' }, type: 'text', required: true, pk: true },
      { field: 'ssn', label: { tr: 'Kişi (TC / SSN)', en: 'Person (SSN)' }, type: 'refSelect', ref: { endpoint: 'People', value: 'ssn', labelFields: ['firstName', 'lastName'] } },
      { field: 'jobTitle', label: { tr: 'Görev', en: 'Job Title' }, type: 'text' },
      { field: 'hireDate', label: { tr: 'İşe Giriş Tarihi', en: 'Hire Date' }, type: 'date' },
      { field: 'salary', label: { tr: 'Maaş', en: 'Salary' }, type: 'number' },
      { field: 'shift', label: { tr: 'Vardiya', en: 'Shift' }, type: 'select', options: ['Day', 'Night', 'Rotating'] },
      { field: 'supervisorEmployeeId', label: { tr: 'Amir', en: 'Supervisor' }, type: 'refSelect', ref: { endpoint: 'Employees', value: 'employeeId', label: 'jobTitle' } },
    ],
  },
  monumentTypes: {
    label: 'nav_monumentTypes', endpoint: 'MonumentTypes', key: ['monumentCode'],
    columns: [
      { field: 'monumentCode', label: { tr: 'Kod', en: 'Code' } },
      { field: 'material', label: { tr: 'Malzeme', en: 'Material' } },
      { field: 'style', label: { tr: 'Stil', en: 'Style' } },
      { field: 'maxHeight', label: { tr: 'Maks. Yükseklik', en: 'Max Height' } },
      { field: 'baseWidth', label: { tr: 'Taban Genişliği', en: 'Base Width' } },
      { field: 'color', label: { tr: 'Renk', en: 'Color' } },
    ],
    fields: [
      { field: 'monumentCode', label: { tr: 'Kod (ör. M011)', en: 'Code' }, type: 'text', required: true, pk: true },
      { field: 'material', label: { tr: 'Malzeme', en: 'Material' }, type: 'text' },
      { field: 'style', label: { tr: 'Stil', en: 'Style' }, type: 'text' },
      { field: 'maxHeight', label: { tr: 'Maks. Yükseklik', en: 'Max Height' }, type: 'number' },
      { field: 'baseWidth', label: { tr: 'Taban Genişliği', en: 'Base Width' }, type: 'number' },
      { field: 'color', label: { tr: 'Renk', en: 'Color' }, type: 'text' },
    ],
  },
  reservations: {
    label: 'nav_reservations', endpoint: 'Reservations', key: ['reservationId'],
    columns: [
      { field: 'reservationId', label: { tr: 'ID', en: 'ID' } },
      { field: 'startDate', label: { tr: 'Başlangıç', en: 'Start' } },
      { field: 'endDate', label: { tr: 'Bitiş', en: 'End' } },
      { field: 'reservationType', label: { tr: 'Tür', en: 'Type' }, badge: true, filter: true },
      { field: 'ownerSsn', label: { tr: 'Sahip', en: 'Owner' } },
      { field: 'plotNumber', label: { tr: 'Parsel', en: 'Plot' } },
    ],
    fields: [
      { field: 'reservationId', label: { tr: 'Rezervasyon ID (ör. RES011)', en: 'Reservation ID' }, type: 'text', required: true, pk: true },
      { field: 'startDate', label: { tr: 'Başlangıç Tarihi', en: 'Start Date' }, type: 'date' },
      { field: 'endDate', label: { tr: 'Bitiş Tarihi', en: 'End Date' }, type: 'date' },
      { field: 'reservationType', label: { tr: 'Tür', en: 'Type' }, type: 'text' },
      { field: 'notes', label: { tr: 'Notlar', en: 'Notes' }, type: 'text' },
      { field: 'ownerSsn', label: { tr: 'Sahip', en: 'Owner' }, type: 'refSelect', ref: { endpoint: 'GraveOwners', value: 'ssn', label: 'ownerType' } },
      { field: 'receiptNo', label: { tr: 'Makbuz', en: 'Receipt' }, type: 'refSelect', ref: { endpoint: 'Payments', value: 'receiptNo', label: 'amount' } },
      { field: 'plotNumber', label: { tr: 'Parsel', en: 'Plot' }, type: 'refSelect', ref: { endpoint: 'GravePlots', value: 'plotNumber', label: 'status' } },
    ],
  },
  funeralServices: {
    label: 'nav_funeralServices', endpoint: 'FuneralServices', key: ['serviceId'],
    columns: [
      { field: 'serviceId', label: { tr: 'ID', en: 'ID' } },
      { field: 'serviceDate', label: { tr: 'Tarih', en: 'Date' } },
      { field: 'startTime', label: { tr: 'Başlangıç', en: 'Start' } },
      { field: 'endTime', label: { tr: 'Bitiş', en: 'End' } },
      { field: 'serviceType', label: { tr: 'Tür', en: 'Type' }, badge: true, filter: true },
      { field: 'deceasedSsn', label: { tr: 'Vefat Eden', en: 'Deceased' } },
    ],
    fields: [
      { field: 'serviceId', label: { tr: 'Servis ID (ör. SRV011)', en: 'Service ID' }, type: 'text', required: true, pk: true },
      { field: 'serviceDate', label: { tr: 'Tarih', en: 'Date' }, type: 'date' },
      { field: 'startTime', label: { tr: 'Başlangıç Saati', en: 'Start Time' }, type: 'time' },
      { field: 'endTime', label: { tr: 'Bitiş Saati', en: 'End Time' }, type: 'time' },
      { field: 'serviceType', label: { tr: 'Tür', en: 'Type' }, type: 'text' },
      { field: 'expectedAttendees', label: { tr: 'Beklenen Katılımcı', en: 'Expected Attendees' }, type: 'number' },
      { field: 'deceasedSsn', label: { tr: 'Vefat Eden (SSN)', en: 'Deceased (SSN)' }, type: 'refSelect', ref: { endpoint: 'DeceasedPeople', value: 'ssn', label: 'religion' } },
    ],
  },
  burialPermits: {
    label: 'nav_burialPermits', endpoint: 'BurialPermits', key: ['permitNumber'],
    columns: [
      { field: 'permitNumber', label: { tr: 'İzin No', en: 'Permit No' } },
      { field: 'issuingAuthority', label: { tr: 'Veren Kurum', en: 'Authority' } },
      { field: 'issueDate', label: { tr: 'Veriliş', en: 'Issued' } },
      { field: 'expirationDate', label: { tr: 'Bitiş', en: 'Expires' } },
      { field: 'authorizedSignature', label: { tr: 'İmza', en: 'Signature' } },
    ],
    fields: [
      { field: 'permitNumber', label: { tr: 'İzin No (ör. PRM014)', en: 'Permit No' }, type: 'text', required: true, pk: true },
      { field: 'issuingAuthority', label: { tr: 'Veren Kurum', en: 'Issuing Authority' }, type: 'text' },
      { field: 'issueDate', label: { tr: 'Veriliş Tarihi', en: 'Issue Date' }, type: 'date' },
      { field: 'expirationDate', label: { tr: 'Bitiş Tarihi', en: 'Expiration Date' }, type: 'date' },
      { field: 'authorizedSignature', label: { tr: 'Yetkili İmza', en: 'Authorized Signature' }, type: 'text' },
    ],
  },
  visitorLogs: {
    label: 'nav_visitorLogs', endpoint: 'VisitorLogs', key: ['visitId'],
    columns: [
      { field: 'visitId', label: { tr: 'ID', en: 'ID' } },
      { field: 'visitorName', label: { tr: 'Ziyaretçi', en: 'Visitor' } },
      { field: 'visitDate', label: { tr: 'Tarih', en: 'Date' } },
      { field: 'arrivalTime', label: { tr: 'Geliş', en: 'Arrival' } },
      { field: 'departureTime', label: { tr: 'Ayrılış', en: 'Departure' } },
      { field: 'plotNumber', label: { tr: 'Parsel', en: 'Plot' } },
    ],
    fields: [
      { field: 'visitId', label: { tr: 'Ziyaret ID (ör. VIS011)', en: 'Visit ID' }, type: 'text', required: true, pk: true },
      { field: 'visitorName', label: { tr: 'Ziyaretçi Adı', en: 'Visitor Name' }, type: 'text' },
      { field: 'visitDate', label: { tr: 'Tarih', en: 'Date' }, type: 'date' },
      { field: 'arrivalTime', label: { tr: 'Geliş Saati', en: 'Arrival Time' }, type: 'time' },
      { field: 'departureTime', label: { tr: 'Ayrılış Saati', en: 'Departure Time' }, type: 'time' },
      { field: 'purpose', label: { tr: 'Amaç', en: 'Purpose' }, type: 'text' },
      { field: 'plotNumber', label: { tr: 'Parsel', en: 'Plot' }, type: 'refSelect', ref: { endpoint: 'GravePlots', value: 'plotNumber', label: 'status' } },
    ],
  },
  users: {
    label: 'nav_users', endpoint: 'Users', key: ['userId'],
    columns: [
      { field: 'userId', label: { tr: 'ID', en: 'ID' } },
      { field: 'username', label: { tr: 'Kullanıcı Adı', en: 'Username' } },
      { field: 'role', label: { tr: 'Rol', en: 'Role' }, badge: true, filter: true },
      { field: 'createdAt', label: { tr: 'Oluşturulma', en: 'Created' } },
    ],
    fields: [
      { field: 'username', label: { tr: 'Kullanıcı Adı', en: 'Username' }, type: 'text', required: true, pk: true },
      { field: 'password', label: { tr: 'Şifre (düzenlemede boş = değişmez)', en: 'Password (blank = unchanged on edit)' }, type: 'password' },
      { field: 'role', label: { tr: 'Rol', en: 'Role' }, type: 'select', options: ['Admin', 'Public'] },
    ],
  },
  auditLogs: {
    label: 'nav_audit', endpoint: 'AuditLogs', readOnly: true, key: ['auditId'],
    columns: [
      { field: 'timestamp', label: { tr: 'Zaman', en: 'Time' } },
      { field: 'username', label: { tr: 'Kullanıcı', en: 'User' }, filter: true },
      { field: 'action', label: { tr: 'İşlem', en: 'Action' }, badge: true, filter: true },
      { field: 'entity', label: { tr: 'Tablo', en: 'Entity' }, filter: true },
      { field: 'entityKey', label: { tr: 'Kayıt', en: 'Record' } },
    ],
  },
};

let currentKey = 'gravePlots';
let currentView = 'home';
let currentData = [];
let currentPage = 1;
let activeFilters = {};
let currentMonths = 0; // 0 = tum zamanlar

const el = (id) => document.getElementById(id);
const fmt = (v) => (v === null || v === undefined || v === '' ? '—' : v);
const keyPath = (cfg, item) => cfg.key.map((k) => item[k]).join('/');

function badgeHtml(value) {
  const v = (value || '').toString();
  let cls = 'bg-surface-container-high text-secondary border-outline-variant';
  if (v === 'Occupied') cls = 'bg-[#f1f5f9] text-[#475569] border-outline-variant';
  else if (v === 'Reserved') cls = 'bg-primary-fixed-dim text-on-primary-fixed border-primary-fixed';
  else if (v === 'Maintenance') cls = 'bg-tertiary-fixed text-tertiary border-tertiary-fixed';
  return `<span class="px-3 py-1 rounded-full text-xs font-semibold border ${cls}">${fmt(valueLabel(v))}</span>`;
}

// --- Istatistik kartlari ---
async function loadStats() {
  try {
    const res = await fetch('/api/Stats?months=' + currentMonths, { headers: authHeaders() });
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
    const res = await fetch('/api/Stats/charts?months=' + currentMonths, { headers: authHeaders() });
    const d = await res.json();
    const dark = document.documentElement.classList.contains('dark');
    const green = dark ? '#6f9f78' : '#4a7c59';
    const greenDim = dark ? '#8bbf94' : '#8ecf9e';
    const slate = dark ? '#8894a8' : '#6b6358', gold = dark ? '#c2a56e' : '#705c30', mid = '#c4a66a';
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
    a.classList.toggle('text-white/80', !active);
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
  el('searchBox').style.display = 'none';
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
  if (s === 'Maintenance') return '#c4a66a';
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
  el('searchBox').style.display = 'none';
  el('btnAdd').style.display = 'none';
  el('pageTitle').textContent = t('nav_map');
  highlightNav('map');

  if (typeof L === 'undefined') return;
  if (!leafletMap) {
    leafletMap = L.map('map').setView([36.8975, 30.6247], 17);
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
        `${statusLabel(p.status)}<br>${t('map_occupant')}: ${p.occupant || t('map_empty')}<br>` +
        `<a href="https://www.google.com/maps/dir/?api=1&destination=${p.latitude},${p.longitude}" target="_blank" rel="noopener" style="color:#4a7c59;font-weight:600;">${t('directions')} ↗</a>`
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
  const events = [];
  try {
    const [fRes, vRes] = await Promise.all([
      fetch('/api/FuneralServices/calendar', { headers: authHeaders() }),
      fetch('/api/VisitorLogs', { headers: authHeaders() }),
    ]);
    const funerals = await fRes.json();
    const visits = await vRes.json();

    funerals.filter((e) => e.serviceDate).forEach((e) => events.push({
      title: t('cal_funeral') + ': ' + (e.deceasedName || e.serviceType || ''),
      start: e.startTime ? `${e.serviceDate}T${e.startTime}` : e.serviceDate,
      end: e.endTime ? `${e.serviceDate}T${e.endTime}` : undefined,
      color: '#4a7c59',
      extendedProps: { kind: 'funeral', serviceType: e.serviceType, attendees: e.expectedAttendees },
    }));

    visits.filter((v) => v.visitDate).forEach((v) => events.push({
      title: t('cal_visit') + ': ' + (v.visitorName || ''),
      start: v.arrivalTime ? `${v.visitDate}T${v.arrivalTime}` : v.visitDate,
      end: v.departureTime ? `${v.visitDate}T${v.departureTime}` : undefined,
      color: '#705c30',
      extendedProps: { kind: 'visit', purpose: v.purpose, plot: v.plotNumber },
    }));
  } catch (e) { /* sessiz */ }
  return events;
}

async function showCalendar() {
  currentView = 'calendar';
  el('homeView').classList.add('hidden');
  el('tableView').classList.add('hidden');
  el('mapView').classList.add('hidden');
  el('calendarView').classList.remove('hidden');
  el('searchBox').style.display = 'none';
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
        if (p.kind === 'visit') {
          toast(`${info.event.title}\n${t('cal_purpose')}: ${p.purpose || '—'}\n${t('cal_plot')}: ${p.plot || '—'}`);
        } else {
          toast(`${info.event.title}\n${t('cal_service')}: ${p.serviceType || '—'}\n${t('cal_attendees')}: ${p.attendees ?? '—'}`);
        }
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
  el('searchBox').style.display = '';
  el('searchInput').value = '';
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
    currentPage = 1;
    buildFilters(cfg);
    renderRows();
  } catch (e) {
    el('tableBody').innerHTML = `<tr><td class="py-8 px-6 text-center text-error" colspan="99">${t('load_error')}</td></tr>`;
  }
}

const PAGE_SIZE = 20;

function changePage(delta) {
  currentPage += delta;
  renderRows();
}

// Filtrelenebilir sutunlar icin ozel acilir menuler (siteyle uyumlu)
function buildFilters(cfg) {
  activeFilters = {};
  const box = el('tableFilters');
  const cols = (cfg.columns || []).filter((c) => c.filter);
  box.innerHTML = cols.map((c) => {
    const vals = [...new Set(currentData
      .map((r) => r[c.field])
      .filter((v) => v !== null && v !== undefined && v !== ''))]
      .sort((a, b) => ('' + a).localeCompare('' + b, 'tr'));
    const opts = [''].concat(vals).map((v) => {
      const label = v === '' ? t('period_all') : valueLabel(v);
      const val = ('' + v).replace(/'/g, "\\'");
      return `<button type="button" onclick="pickFilter('${c.field}','${val}')" class="w-full text-left px-4 py-2.5 text-sm hover:bg-surface-container-low">${label}</button>`;
    }).join('');
    return `<div class="relative">
      <button type="button" onclick="toggleFilterMenu('${c.field}')" class="flex items-center gap-2 px-4 py-2.5 bg-surface-container-lowest border border-outline-variant rounded-full text-sm hover:border-primary transition-colors">
        <span class="text-on-surface-variant">${lbl(c.label)}:</span>
        <span id="fltval_${c.field}" class="font-semibold">${t('period_all')}</span>
        <span class="material-symbols-outlined text-[18px] text-on-surface-variant">expand_more</span>
      </button>
      <div id="fltmenu_${c.field}" class="hidden absolute left-0 mt-1 min-w-[180px] max-h-64 overflow-y-auto bg-surface-container-lowest border border-outline-variant rounded-xl shadow-lg z-[500]">${opts}</div>
    </div>`;
  }).join('');
}

function toggleFilterMenu(field) {
  const m = document.getElementById('fltmenu_' + field);
  document.querySelectorAll('[id^="fltmenu_"]').forEach((x) => { if (x !== m) x.classList.add('hidden'); });
  m.classList.toggle('hidden');
}

function pickFilter(field, value) {
  if (value === '') delete activeFilters[field];
  else activeFilters[field] = value;
  const lblEl = document.getElementById('fltval_' + field);
  if (lblEl) lblEl.textContent = value === '' ? t('period_all') : valueLabel(value);
  const m = document.getElementById('fltmenu_' + field);
  if (m) m.classList.add('hidden');
  currentPage = 1;
  renderRows();
}

function renderRows() {
  const cfg = ENTITIES[currentKey];
  const q = el('searchInput').value.trim().toLowerCase();
  let rows = currentData;

  // Sutun filtreleri
  Object.entries(activeFilters).forEach(([f, v]) => {
    rows = rows.filter((item) => (item[f] ?? '').toString() === v);
  });

  // Metin aramasi
  if (q) {
    rows = rows.filter((item) =>
      cfg.columns.some((c) => (item[c.field] ?? '').toString().toLowerCase().includes(q))
    );
  }

  const total = rows.length;
  const pages = Math.max(1, Math.ceil(total / PAGE_SIZE));
  if (currentPage > pages) currentPage = pages;
  if (currentPage < 1) currentPage = 1;
  const start = (currentPage - 1) * PAGE_SIZE;
  const pageRows = rows.slice(start, start + PAGE_SIZE);

  if (!total) {
    el('tableBody').innerHTML = `<tr><td class="py-8 px-6 text-center text-secondary" colspan="99">${t('no_records')}</td></tr>`;
    el('pageInfo').textContent = t('total') + ': 0';
    el('pageLabel').textContent = '';
    el('prevPage').disabled = true;
    el('nextPage').disabled = true;
    return;
  }

  el('tableBody').innerHTML = pageRows.map((item) => {
    const cells = cfg.columns.map((c) => {
      const val = item[c.field];
      if (c.badge) return `<td class="py-4 px-6">${badgeHtml(val)}</td>`;
      return `<td class="py-4 px-6">${fmt(valueLabel(val))}</td>`;
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

  el('pageInfo').textContent = `${t('shown')}: ${start + 1}-${start + pageRows.length} / ${t('total')}: ${total}`;
  el('pageLabel').textContent = `${currentPage} / ${pages}`;
  el('prevPage').disabled = currentPage <= 1;
  el('nextPage').disabled = currentPage >= pages;
}

// --- Modal / Form ---
// Vefat etmemis kayitli kisileri getir (personSelect icin - artik kullanilmiyor ama dursun)
async function fetchAvailablePersons() {
  try {
    const [pRes, dRes] = await Promise.all([
      fetch('/api/People', { headers: authHeaders() }),
      fetch('/api/DeceasedPeople', { headers: authHeaders() }),
    ]);
    const people = await pRes.json();
    const deceased = await dRes.json();
    const dset = new Set(deceased.map((d) => d.ssn));
    return people.filter((p) => !dset.has(p.ssn));
  } catch (e) { return []; }
}

// Bosta olan (bir vefat edene atanmamis) parselleri getir
async function fetchAvailablePlots(currentVal) {
  try {
    const [gRes, dRes] = await Promise.all([
      fetch('/api/GravePlots', { headers: authHeaders() }),
      fetch('/api/DeceasedPeople', { headers: authHeaders() }),
    ]);
    const plots = await gRes.json();
    const dec = await dRes.json();
    const used = new Set(dec.map((d) => d.plotNumber).filter(Boolean));
    return plots.map((p) => p.plotNumber).filter((pn) => !used.has(pn) || pn === currentVal);
  } catch (e) { return []; }
}

// Kullanilmamis defin izinlerini getir
async function fetchAvailablePermits(currentVal) {
  try {
    const [bRes, dRes] = await Promise.all([
      fetch('/api/BurialPermits', { headers: authHeaders() }),
      fetch('/api/DeceasedPeople', { headers: authHeaders() }),
    ]);
    const permits = await bRes.json();
    const dec = await dRes.json();
    const used = new Set(dec.map((d) => d.permitNumber).filter(Boolean));
    return permits.map((p) => p.permitNumber).filter((pn) => !used.has(pn) || pn === currentVal);
  } catch (e) { return []; }
}

// Bir referans kaydinin etiketini olustur (ör. "Ad Soyad")
function buildRefLabel(d, ref) {
  if (ref.labelFields) return ref.labelFields.map((k) => d[k] ?? '').join(' ').trim();
  return ref.label ? (d[ref.label] ?? '') : '';
}

// Formdaki secici alanlar icin secenekleri hazirla
async function prepareFormOptions(cfg, item) {
  const fields = cfg.fields || [];
  const types = fields.map((f) => f.type);
  if (types.includes('personSelect')) window.__personOptions = await fetchAvailablePersons();
  if (types.includes('plotSelect')) window.__plotOptions = await fetchAvailablePlots(item ? item.plotNumber : null);
  if (types.includes('permitSelect')) window.__permitOptions = await fetchAvailablePermits(item ? item.permitNumber : null);

  // refSelect alanlari: her birinin kaynak tablosunu cek
  const refFields = fields.filter((f) => f.type === 'refSelect');
  if (refFields.length) {
    window.__refOptions = {};
    await Promise.all(refFields.map(async (f) => {
      try {
        const res = await fetch('/api/' + f.ref.endpoint, { headers: authHeaders() });
        const data = await res.json();
        window.__refOptions[f.field] = data.map((d) => ({ value: d[f.ref.value], label: buildRefLabel(d, f.ref) }));
      } catch (e) { window.__refOptions[f.field] = []; }
    }));
  }
}

function buildForm(cfg, item) {
  return cfg.fields.map((f) => {
    const val = item ? (item[f.field] ?? '') : '';
    const disabled = item && f.pk ? 'disabled' : '';
    let input;
    if (f.type === 'personSelect') {
      if (item) {
        input = `<input id="f_${f.field}" type="text" value="${val}" disabled class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg disabled:opacity-60"/>`;
      } else {
        const opts = window.__personOptions || [];
        input = `<select id="f_${f.field}" class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg focus:outline-none focus:ring-2 focus:ring-primary appearance-none cursor-pointer">
          <option value="">${t('select')}</option>
          ${opts.map((o) => `<option value="${o.ssn}">${o.ssn} — ${(o.firstName || '')} ${(o.lastName || '')}${o.dateOfBirth ? ' (' + o.dateOfBirth + ')' : ''}</option>`).join('')}
        </select>`;
        if (!opts.length) input += `<p class="text-xs text-error mt-1">${t('no_available_person')}</p>`;
      }
    } else if (f.type === 'refSelect') {
      if (item && f.pk) {
        input = `<input id="f_${f.field}" type="text" value="${val}" disabled class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg disabled:opacity-60"/>`;
      } else {
        const opts = (window.__refOptions && window.__refOptions[f.field]) || [];
        const list = [...opts];
        if (val && !list.some((o) => o.value === val)) list.unshift({ value: val, label: '' });
        input = `<select id="f_${f.field}" ${f.required ? 'required' : ''} class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg focus:outline-none focus:ring-2 focus:ring-primary appearance-none cursor-pointer">
          <option value="">${t('select')}</option>
          ${list.map((o) => `<option value="${o.value}" ${o.value === val ? 'selected' : ''}>${o.value}${o.label ? (' — ' + o.label) : ''}</option>`).join('')}
        </select>`;
      }
    } else if (f.type === 'plotSelect' || f.type === 'permitSelect') {
      const opts = (f.type === 'plotSelect' ? window.__plotOptions : window.__permitOptions) || [];
      const list = [...opts];
      if (val && !list.includes(val)) list.unshift(val);
      input = `<select id="f_${f.field}" class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg focus:outline-none focus:ring-2 focus:ring-primary appearance-none cursor-pointer">
        <option value="">— (${t('select')})</option>
        ${list.map((o) => `<option value="${o}" ${o === val ? 'selected' : ''}>${o}</option>`).join('')}
      </select>`;
    } else if (f.type === 'select') {
      input = `<select id="f_${f.field}" ${disabled} class="w-full px-4 py-3 bg-surface-container-lowest border border-outline-variant rounded-lg focus:outline-none focus:ring-2 focus:ring-primary appearance-none cursor-pointer disabled:opacity-60">
        <option value="">${t('select')}</option>
        ${f.options.map((o) => `<option value="${o}" ${o === val ? 'selected' : ''}>${valueLabel(o)}</option>`).join('')}
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

async function openAdd() {
  const cfg = ENTITIES[currentKey];
  if (cfg.readOnly) return;
  await prepareFormOptions(cfg, null);
  editingId = null;
  el('modalTitle').textContent = t('modal_add');
  el('formFields').innerHTML = buildForm(cfg, null);
  el('modalError').classList.add('hidden');
  el('modal').classList.remove('hidden');
}

async function openEdit(id) {
  const cfg = ENTITIES[currentKey];
  let item;
  // writeEndpoint varsa (ör. Vefat Edenler) gercek kaydi cekip formu doldur
  if (cfg.writeEndpoint) {
    try {
      const res = await fetch('/api/' + cfg.writeEndpoint + '/' + id, { headers: authHeaders() });
      if (res.ok) item = await res.json();
    } catch (e) { /* yoksay */ }
  }
  if (!item) item = currentData.find((x) => keyPath(cfg, x) === id);
  if (!item) return;
  await prepareFormOptions(cfg, item);
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
    if (f.type === 'time' && /^\d{2}:\d{2}$/.test(v)) v += ':00'; // TimeOnly icin saniye ekle
    body[f.field] = f.type === 'number' ? parseFloat(v) : v;
  });

  const isEdit = editingId !== null;
  const ep = cfg.writeEndpoint || cfg.endpoint;
  const url = '/api/' + ep + (isEdit ? '/' + editingId : '');
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
    toast(t('toast_saved'));
  } catch (e) {
    showModalError(t('err_generic'));
  }
}

async function deleteRow(id) {
  const cfg = ENTITIES[currentKey];
  if (!(await confirmDialog(t('confirm_delete')))) return;
  try {
    const res = await fetch('/api/' + (cfg.writeEndpoint || cfg.endpoint) + '/' + id, { method: 'DELETE', headers: authHeaders() });
    if (res.status === 401 || res.status === 403) { toast(t('err_auth'), 'error'); return; }
    if (!res.ok) {
      const txt = await res.text();
      toast(txt.includes('REFERENCE') || txt.includes('FOREIGN KEY') ? t('delete_fk') : t('err_generic'), 'error');
      return;
    }
    await loadEntity(currentKey);
    await loadStats();
    toast(t('toast_deleted'));
  } catch (e) { toast(t('err_generic'), 'error'); }
}

// Aktif tabloyu (arama filtresi uygulanmis) Excel'e aktar
function exportExcel() {
  const cfg = ENTITIES[currentKey];
  if (typeof XLSX === 'undefined') return;

  const q = el('searchInput').value.trim().toLowerCase();
  let data = currentData;
  Object.entries(activeFilters).forEach(([f, v]) => {
    data = data.filter((item) => (item[f] ?? '').toString() === v);
  });
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

// Odeme makbuzu PDF olustur (gomulu Turkce font ile)
function downloadReceipt(id) {
  const p = currentData.find((x) => x.receiptNo === id);
  if (!p || !window.jspdf) return;
  const tr = LANG === 'tr';
  const doc = new window.jspdf.jsPDF();

  // Gomulu Turkce destekli font (yuklendiyse)
  const hasTrFont = (doc.getFontList && doc.getFontList().Liberation);
  const FONT = hasTrFont ? 'Liberation' : 'helvetica';
  doc.setFont(FONT, 'normal');

  doc.setFontSize(16);
  doc.text(tr ? 'Mezarlık Yönetim Sistemi' : 'Cemetery Management System', 20, 22);
  doc.setFontSize(12);
  doc.text(tr ? 'Ödeme Makbuzu' : 'Payment Receipt', 20, 32);
  doc.setLineWidth(0.3);
  doc.line(20, 36, 190, 36);

  const rows = [
    [tr ? 'Makbuz No' : 'Receipt No', p.receiptNo],
    [tr ? 'Tutar' : 'Amount', (p.amount ?? '') + ' ' + (p.currency || '')],
    [tr ? 'Tarih' : 'Date', p.paymentDate || ''],
    [tr ? 'Ödeme Yöntemi' : 'Method', p.paymentMethod || ''],
    [tr ? 'Sahip SSN' : 'Owner SSN', p.ownerSsn || ''],
    [tr ? 'Fatura Adresi' : 'Billing Address', p.billingAddress || ''],
  ];

  let y = 50;
  doc.setFontSize(11);
  rows.forEach(([k, v]) => {
    doc.setFont(FONT, 'bold');
    doc.text(k + ':', 20, y);
    doc.setFont(FONT, 'normal');
    doc.text(String(v ?? ''), 75, y);
    y += 11;
  });

  doc.setFontSize(9);
  doc.text((tr ? 'Oluşturulma: ' : 'Generated: ') +
    new Date().toLocaleString(tr ? 'tr-TR' : 'en-US'), 20, y + 12);

  doc.save('makbuz_' + p.receiptNo + '.pdf');
}

function showModalError(msg) {
  const box = el('modalError');
  box.textContent = msg;
  box.classList.remove('hidden');
}

// Bildirim (toast) - native alert yerine
function toast(message, type = 'info') {
  const box = el('toast');
  if (!box) return;
  const div = document.createElement('div');
  div.className = (type === 'error' ? 'bg-error text-white' : 'bg-primary text-on-primary') +
    ' px-4 py-3 rounded-xl shadow-lg text-sm font-semibold max-w-xs toast-in';
  div.style.whiteSpace = 'pre-line';
  div.textContent = message;
  box.appendChild(div);
  setTimeout(() => {
    div.style.transition = 'opacity .3s';
    div.style.opacity = '0';
    setTimeout(() => div.remove(), 300);
  }, 3200);
}

// Ozel onay penceresi - native confirm yerine
function confirmDialog(message) {
  return new Promise((resolve) => {
    const modal = el('confirmModal');
    el('confirmMsg').textContent = message;
    modal.classList.remove('hidden');
    const ok = el('confirmOk');
    const cancel = el('confirmCancel');
    const done = (val) => { modal.classList.add('hidden'); ok.onclick = null; cancel.onclick = null; resolve(val); };
    ok.onclick = () => done(true);
    cancel.onclick = () => done(false);
  });
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
    a.addEventListener('click', (e) => { e.preventDefault(); loadEntity(a.dataset.entity); closeSidebar(); });
  });
  document.querySelectorAll('nav a[data-view]').forEach((a) => {
    a.addEventListener('click', (e) => {
      e.preventDefault();
      if (a.dataset.view === 'map') showMap();
      else if (a.dataset.view === 'calendar') showCalendar();
      else showHome();
      closeSidebar();
    });
  });
  el('searchInput').addEventListener('input', () => { currentPage = 1; renderRows(); });
  // Filtre menusu disina tiklaninca kapat
  document.addEventListener('click', (e) => {
    if (!e.target.closest('#tableFilters')) {
      document.querySelectorAll('[id^="fltmenu_"]').forEach((x) => x.classList.add('hidden'));
    }
  });
  el('btnAdd').addEventListener('click', openAdd);
  el('btnSave').addEventListener('click', saveRecord);
  el('btnCancel').addEventListener('click', closeModal);
  el('btnClose').addEventListener('click', closeModal);
  el('btnLogout').addEventListener('click', logout);

  showHome();
});
