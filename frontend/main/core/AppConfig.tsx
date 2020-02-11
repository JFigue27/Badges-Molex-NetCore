//PRODUCTION:
let BaseURL = '%%BASEURL%%';
let UniversalCatalogsURL = '%%CATALOGSURL%%';
let AuthURL = '%%AUTHURL%%';

//DEVELOPMENT:
let Dev_BaseURL = 'http://localhost:5000/';
let Dev_UniversalCatalogsURL = 'http://localhost:5000/';
let Dev_AuthURL = 'https://apps.capsonic.com/Auth/';

//
BaseURL = BaseURL == '%%BASEURL%%' ? Dev_BaseURL : BaseURL;
UniversalCatalogsURL = UniversalCatalogsURL == '%%CATALOGSURL%%' ? Dev_UniversalCatalogsURL : UniversalCatalogsURL;
AuthURL = AuthURL == '%%AUTHURL%%' ? Dev_AuthURL : AuthURL;

export default {
  BaseURL,
  UniversalCatalogsURL,
  AuthURL
};
