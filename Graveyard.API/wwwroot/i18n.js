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
    save: 'Kaydet',
    loading: 'Yükleniyor…',
    no_records: 'Kayıt bulunamadı.',
    load_error: 'Veri alınamadı.',
    confirm_delete: 'Bu kaydı silmek istediğinize emin misiniz?',
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
    nav_gravePlots: 'Mezar Kayıtları',
    nav_burialRecords: 'Vefat Edenler',
    nav_graveOwners: 'Sahiplik Durumları',
    nav_cemeteryZones: 'Bölgeler',
    nav_payments: 'Ödemeler',
    nav_maintenanceLogs: 'Bakım Kayıtları',
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
    save: 'Save',
    loading: 'Loading…',
    no_records: 'No records found.',
    load_error: 'Failed to load data.',
    confirm_delete: 'Are you sure you want to delete this record?',
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
    nav_gravePlots: 'Grave Records',
    nav_burialRecords: 'Deceased',
    nav_graveOwners: 'Ownerships',
    nav_cemeteryZones: 'Zones',
    nav_payments: 'Payments',
    nav_maintenanceLogs: 'Maintenance Logs',
  },
};

let LANG = localStorage.getItem('lang') || 'tr';

function t(key) {
  return (I18N[LANG] && I18N[LANG][key]) || (I18N.tr[key]) || key;
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
