/* ================= i18n: TR / EN dil altyapisi ================= */
const I18N = {
  tr: {
    app_title: 'Mezarlık Sistemi',
    app_subtitle: 'Belediye Yönetim Paneli',
    login_title: 'Mezarlık Yönetimi',
    login_subtitle: 'Lütfen yönetici paneline erişmek için giriş yapın.',
    username: 'Kullanıcı Adı',
    password: 'Şifre',
    login_btn: 'Giriş Yap',
    login_footer1: 'Güvenli Belediye Yönetim Sistemi altyapısıdır.',
    login_footer2: 'Yetkisiz erişim yasaktır.',
    login_error: 'Kullanıcı adı veya şifre hatalı.',
    login_conn_error: 'Sunucuya bağlanılamadı. API çalışıyor mu?',
    username_ph: 'Kullanıcı adınızı girin',
    search_ph: 'Ara...',
    logout: 'Çıkış Yap',
    overview: 'Genel Bakış',
    add_new: 'Yeni Kayıt Ekle',
    stat_total_plots: 'Toplam Mezar Sayısı',
    stat_occupancy: 'Doluluk Oranı',
    stat_deceased: 'Toplam Vefat Eden',
    stat_revenue: 'Toplam Gelir',
    modal_add: 'Yeni Kayıt Ekle',
    modal_edit: 'Kaydı Düzenle',
    cancel: 'İptal',
    confirm_ok: 'Sil',
    save: 'Kaydet',
    toast_saved: 'Kaydedildi',
    toast_deleted: 'Silindi',
    toast_archived: 'Arşive taşındı',
    toast_restored: 'Arşivden geri getirildi',
    loading: 'Yükleniyor…',
    no_records: 'Kayıt bulunamadı.',
    load_error: 'Veri alınamadı.',
    confirm_delete: 'Bu kaydı silmek istediğinize emin misiniz?',
    confirm_archive: 'Bu kayıt arşive taşınsın mı? (İstediğinizde geri getirebilirsiniz.)',
    confirm_purge: 'Bu kayıt KALICI olarak silinsin mi? Bu işlem geri alınamaz!',
    archive: 'Arşivle',
    restore: 'Geri Al',
    purge: 'Kalıcı Sil',
    show_archived: 'Arşivi Göster',
    show_active: 'Aktifleri Göster',
    shown: 'Gösterilen',
    total: 'Toplam',
    select: 'Seçiniz',
    actions: 'İşlemler',
    delete_fk: 'Bu kayıt başka kayıtlara bağlı olduğu için silinemez.',
    err_duplicate: 'Bu anahtar (numara/ID) zaten kayıtlı. Farklı bir değer girin.',
    err_fk: 'İlişkili kayıt bulunamadı. Girdiğiniz bir referans sistemde yok.',
    err_check: 'Girilen değer kurallara uymuyor (ör. tarih veya tutar geçersiz).',
    err_null: 'Zorunlu bir alan boş bırakılmış.',
    err_generic: 'Kaydedilemedi. Lütfen alanları kontrol edip tekrar deneyin.',
    err_auth: 'Bu işlem için admin yetkisi gerekli. Lütfen tekrar giriş yapın.',
    nav_home: 'Ana Sayfa',
    nav_people: 'Kişiler',
    nav_gravePlots: 'Mezar Kayıtları',
    nav_burialRecords: 'Vefat Edenler',
    nav_graveOwners: 'Sahiplik Durumları',
    nav_cemeteries: 'Mezarlıklar',
    nav_cemeteryZones: 'Bölgeler',
    nav_payments: 'Ödemeler',
    nav_maintenanceLogs: 'Bakım Kayıtları',
    nav_employees: 'Çalışanlar',
    nav_monumentTypes: 'Anıt Tipleri',
    nav_reservations: 'Rezervasyonlar',
    nav_funeralServices: 'Cenaze Hizmetleri',
    nav_burialPermits: 'Defin İzinleri',
    nav_visitorLogs: 'Ziyaretçi Kayıtları',
    nav_users: 'Kullanıcılar',
    nav_audit: 'İşlem Günlüğü',
    chart_zone: 'Bölgelere Göre Doluluk',
    chart_deaths: 'Aylara Göre Vefat',
    chart_payments: 'Ödeme Yöntemleri',
    chart_finance: 'Gelir / Gider',
    chart_maint: 'Aylara Göre Bakım Maliyeti',
    chart_visits: 'En Yoğun Ziyaret Günleri',
    chart_zone_pct: 'Bölge Bazlı Doluluk Oranı',
    pick_on_map: 'Haritadan Seç',
    pick_on_map_hint: 'Haritaya tıklayın veya işaretçiyi sürükleyin — Enlem/Boylam otomatik dolar.',
    stat_expense: 'Toplam Gider',
    stat_net: 'Net Bakiye',
    fin_income: 'Gelir',
    fin_expense: 'Gider',
    fin_net: 'Net',
    period_all: 'Tümü',
    period_1m: 'Son 1 Ay',
    period_3m: 'Son 3 Ay',
    period_6m: 'Son 6 Ay',
    period_1y: 'Son 1 Yıl',
    nav_map: 'Harita',
    nav_calendar: 'Takvim',
    cal_service: 'Hizmet',
    cal_attendees: 'Beklenen Katılımcı',
    cal_funeral: 'Cenaze Hizmeti',
    cal_visit: 'Ziyaret',
    cal_purpose: 'Amaç',
    cal_plot: 'Parsel',
    map_zone: 'Bölge',
    map_occupant: 'Yatan Kişi',
    map_empty: 'Boş',
    st_Available: 'Boş',
    st_Occupied: 'Dolu',
    st_Reserved: 'Rezerve',
    st_Maintenance: 'Bakım Gerektiriyor',
    values: {
      Available: 'Boş', Occupied: 'Dolu', Reserved: 'Rezerve', Maintenance: 'Bakım',
      Cash: 'Nakit', Card: 'Kart', Transfer: 'Havale',
      Individual: 'Bireysel', Family: 'Aile', Institution: 'Kurum',
      Day: 'Gündüz', Night: 'Gece', Rotating: 'Dönüşümlü',
      Male: 'Erkek', Female: 'Kadın', Unknown: 'Bilinmiyor',
      Yes: 'Evet', No: 'Hayır',
      Permanent: 'Kalıcı', Temporary: 'Geçici',
      Islamic: 'İslami', Christian: 'Hristiyan', Veteran: 'Gazi', Memorial: 'Anma',
      Admin: 'Yönetici', Public: 'Halka Açık',
      Create: 'Ekleme', Update: 'Güncelleme', Delete: 'Silme',
      Islam: 'İslam', Musevi: 'Musevi',
    },
    pub_title: 'Mezar Yeri Sorgulama',
    pub_subtitle: 'Vefat eden yakınınızın adını girerek mezar konumunu bulun.',
    pub_search_ph: 'Ad soyad ile arayın...',
    pub_search_btn: 'Ara',
    pub_no_result: 'Sonuç bulunamadı.',
    pub_hint: 'Aramak için bir isim yazın.',
    pub_admin: 'Giriş Yap',
    pub_show_map: 'Haritada Göster',
    pub_plot: 'Parsel',
    directions: 'Yol Tarifi',
    pub_year: 'Ölüm Yılı',
    pub_announcements: 'Son Vefat Edenler',
    pub_year_clear: 'Temizle',
    excel_export: "Excel'e Aktar",
    no_available_person: 'Uygun kişi yok. Önce Kişiler sekmesinden kişi ekleyin.',
  },
  en: {
    app_title: 'Cemetery System',
    app_subtitle: 'Municipal Management Panel',
    login_title: 'Cemetery Management',
    login_subtitle: 'Please sign in to access the admin panel.',
    username: 'Username',
    password: 'Password',
    login_btn: 'Sign In',
    login_footer1: 'Secure Municipal Management System infrastructure.',
    login_footer2: 'Unauthorized access is prohibited.',
    login_error: 'Invalid username or password.',
    login_conn_error: 'Could not reach the server. Is the API running?',
    username_ph: 'Enter your username',
    search_ph: 'Search...',
    logout: 'Sign Out',
    overview: 'Overview',
    add_new: 'Add New Record',
    stat_total_plots: 'Total Plots',
    stat_occupancy: 'Occupancy Rate',
    stat_deceased: 'Total Deceased',
    stat_revenue: 'Total Revenue',
    modal_add: 'Add New Record',
    modal_edit: 'Edit Record',
    cancel: 'Cancel',
    confirm_ok: 'Delete',
    save: 'Save',
    toast_saved: 'Saved',
    toast_deleted: 'Deleted',
    toast_archived: 'Moved to archive',
    toast_restored: 'Restored from archive',
    loading: 'Loading…',
    no_records: 'No records found.',
    load_error: 'Failed to load data.',
    confirm_delete: 'Are you sure you want to delete this record?',
    confirm_archive: 'Move this record to the archive? (You can restore it anytime.)',
    confirm_purge: 'Permanently delete this record? This cannot be undone!',
    archive: 'Archive',
    restore: 'Restore',
    purge: 'Delete Permanently',
    show_archived: 'Show Archived',
    show_active: 'Show Active',
    shown: 'Showing',
    total: 'Total',
    select: 'Select',
    actions: 'Actions',
    delete_fk: 'This record cannot be deleted because other records depend on it.',
    err_duplicate: 'This key (number/ID) already exists. Please enter a different value.',
    err_fk: 'Related record not found. A reference you entered does not exist.',
    err_check: 'The value violates a rule (e.g. invalid date or amount).',
    err_null: 'A required field is empty.',
    err_generic: 'Could not save. Please check the fields and try again.',
    err_auth: 'Admin privileges are required for this action. Please sign in again.',
    nav_home: 'Home',
    nav_people: 'People',
    nav_gravePlots: 'Grave Records',
    nav_burialRecords: 'Deceased',
    nav_graveOwners: 'Ownerships',
    nav_cemeteries: 'Cemeteries',
    nav_cemeteryZones: 'Zones',
    nav_payments: 'Payments',
    nav_maintenanceLogs: 'Maintenance Logs',
    nav_employees: 'Employees',
    nav_monumentTypes: 'Monument Types',
    nav_reservations: 'Reservations',
    nav_funeralServices: 'Funeral Services',
    nav_burialPermits: 'Burial Permits',
    nav_visitorLogs: 'Visitor Logs',
    nav_users: 'Users',
    nav_audit: 'Audit Log',
    chart_zone: 'Occupancy by Zone',
    chart_deaths: 'Deaths by Month',
    chart_payments: 'Payment Methods',
    chart_finance: 'Income / Expense',
    chart_maint: 'Maintenance Cost by Month',
    chart_visits: 'Busiest Visit Days',
    chart_zone_pct: 'Occupancy Rate by Zone',
    pick_on_map: 'Pick on Map',
    pick_on_map_hint: 'Click the map or drag the marker — Latitude/Longitude fill in automatically.',
    stat_expense: 'Total Expense',
    stat_net: 'Net Balance',
    fin_income: 'Income',
    fin_expense: 'Expense',
    fin_net: 'Net',
    period_all: 'All',
    period_1m: 'Last 1 Month',
    period_3m: 'Last 3 Months',
    period_6m: 'Last 6 Months',
    period_1y: 'Last 1 Year',
    nav_map: 'Map',
    nav_calendar: 'Calendar',
    cal_service: 'Service',
    cal_attendees: 'Expected Attendees',
    cal_funeral: 'Funeral Service',
    cal_visit: 'Visit',
    cal_purpose: 'Purpose',
    cal_plot: 'Plot',
    map_zone: 'Zone',
    map_occupant: 'Occupant',
    map_empty: 'Empty',
    st_Available: 'Available',
    st_Occupied: 'Occupied',
    st_Reserved: 'Reserved',
    st_Maintenance: 'Maintenance',
    pub_title: 'Find a Grave',
    pub_subtitle: 'Enter the name of your loved one to find the grave location.',
    pub_search_ph: 'Search by full name...',
    pub_search_btn: 'Search',
    pub_no_result: 'No results found.',
    pub_hint: 'Type a name to search.',
    pub_admin: 'Login',
    pub_show_map: 'Show on Map',
    pub_plot: 'Plot',
    directions: 'Directions',
    pub_year: 'Death Year',
    pub_announcements: 'Recent Deaths',
    pub_year_clear: 'Clear',
    excel_export: 'Export to Excel',
    no_available_person: 'No available person. Add one in the People tab first.',
  },
};

let LANG = localStorage.getItem('lang') || 'tr';

function t(key) {
  return (I18N[LANG] && I18N[LANG][key]) || (I18N.tr[key]) || key;
}

// Veri degerlerini (Available, Card, Male...) dile gore cevir; bilinmiyorsa oldugu gibi birak
function valueLabel(v) {
  if (v === null || v === undefined || v === '') return v;
  const map = I18N[LANG] && I18N[LANG].values;
  return (map && map['' + v]) || v;
}

const LANG_NAMES = { tr: 'Türkçe', en: 'English' };

// data-i18n / data-i18n-ph tasiyan tum ogeleri cevir
function applyI18n() {
  document.querySelectorAll('[data-i18n]').forEach((e) => { e.textContent = t(e.dataset.i18n); });
  document.querySelectorAll('[data-i18n-ph]').forEach((e) => { e.placeholder = t(e.dataset.i18nPh); });
  document.documentElement.lang = LANG;
  const cur = document.getElementById('langCurrent');
  if (cur) cur.textContent = LANG_NAMES[LANG];
  // Menudeki aktif secenegi isaretle
  document.querySelectorAll('[data-lang-opt]').forEach((b) => {
    b.classList.toggle('bg-surface-container-low', b.dataset.langOpt === LANG);
    b.classList.toggle('font-semibold', b.dataset.langOpt === LANG);
  });
}

function setLang(l) {
  LANG = l;
  localStorage.setItem('lang', l);
  applyI18n();
  if (typeof window.onLangChange === 'function') window.onLangChange();
}

// Acilir dil menusu
function toggleLangMenu() {
  const m = document.getElementById('langMenu');
  if (m) m.classList.toggle('hidden');
}

function chooseLang(l) {
  setLang(l);
  const m = document.getElementById('langMenu');
  if (m) m.classList.add('hidden');
}

// Dis tiklamada menuyu kapat
document.addEventListener('click', (e) => {
  const btn = document.getElementById('langBtn');
  const menu = document.getElementById('langMenu');
  if (menu && btn && !btn.contains(e.target) && !menu.contains(e.target)) menu.classList.add('hidden');
});
