import 'isomorphic-fetch';
import AppConfig from './AppConfig';
import { IAuth } from './Contract';

const request = async (method, endpoint) => {
  return await fetch(AppConfig.AuthURL + endpoint, {
    method: method,
    mode: 'cors',
    cache: 'no-cache',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded'
    }
  });
};

export default class AuthService {
  static auth?: IAuth;

  static fillAuthData = () => {
    AuthService.auth = JSON.parse(localStorage.getItem('auth') || 'null');
    return AuthService.auth;
  };

  static login = async (username, password) => {
    let data = `username=${username}&password=${password}`;
    try {
      let response = await request('POST', 'auth/credentials.json?' + data);
      if (!response.ok) throw 'Invalid';
      let userAuth = await response.json();
      AuthService.auth = {
        user: userAuth
      };
      localStorage.setItem('auth', JSON.stringify(AuthService.auth));
      // AuthService.ON_LOGIN();
      return AuthService.auth;
    } catch (e) {
      console.log(e);
      throw 'Invalid Username or Password.';
    }
  };

  static logout = async () => {
    localStorage.removeItem('auth');
    AuthService.auth = undefined;
    await request('GET', 'auth/logout').then(r => {
      console.log(r);
    });
  };

  static ON_LOGIN = () => {};

  static OpenLogin = () => {};
}
