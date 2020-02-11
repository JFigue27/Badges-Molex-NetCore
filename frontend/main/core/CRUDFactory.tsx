import 'isomorphic-fetch';
import moment from 'moment';
import AppConfig from './AppConfig';
import AuthService from '../core/AuthService';

const GeneralError = response => {
  //CommonResponse wrapper
  if (response.ErrorThrown) {
    switch (response.ErrorType) {
      case 'MESSAGE':
        alert(response.ResponseDescription);
    }
    throw response.ResponseDescription;
  }
  //ServiceStack wrapper
  else if (response.ResponseStatus) {
    switch (response.ResponseStatus.ErrorCode) {
      case 'KnownError':
      case 'SqlException':
      default:
        console.log(response);
        console.log(response.ResponseStatus.StackTrace);
        alert(response.ResponseStatus.Message);
    }
    throw response.ResponseStatus.Message;
  }
  //Other
  else {
    switch (response.status) {
      case 401:
        AuthService.OpenLogin();
        throw 'Your session has expired. Log in again';
    }
  }
  throw response;
};

const Request = async (method: string, endpoint: string, data: any, BaseURL?: string) => {
  if (AuthService.auth == null) AuthService.fillAuthData();
  if (!AuthService.auth || !AuthService.auth.user) throw 'User not signed in.';

  const config: RequestInit = {
    method: method,
    mode: 'cors',
    // cache: 'no-cache',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${AuthService.auth.user.BearerToken}`
    },
    body: null
  };
  if (['POST', 'PUT', 'DELETE'].includes(method)) config.body = JSON.stringify(data);
  let response = await fetch((BaseURL || AppConfig.BaseURL) + endpoint, config);
  if (response) {
    if (!response.ok) throw await response.json();
    if (response.status == 403) alert('Invalid Role.');
    if (response.status == 401) throw response;
  } else {
    alert('Failed to fetch. Probably server is down.');
  }
  try {
    const result = await response.json();
    return result;
  } catch (e) {
    return null;
  }
};

//Short Aliases to Request calls:
const Get = async (endpoint: string, baseURL?: string) => await Request('GET', endpoint, null, baseURL);
const Post = async (endpoint: string, data?: any, baseURL?: string) => await Request('POST', endpoint, data, baseURL);
const Put = async (endpoint: string, data?: any, baseURL?: string) => await Request('PUT', endpoint, data, baseURL);
const Delete = async (endpoint: string, data?: any, baseURL?: string) => await Request('DELETE', endpoint, data, baseURL);

interface IConfigCRUDFactory {
  EndPoint: string;
  BaseURL?: string;
}

export class CRUDFactory {
  EndPoint: string;
  BaseURL: string;

  constructor({ EndPoint, BaseURL = AppConfig.BaseURL }: IConfigCRUDFactory) {
    this.EndPoint = EndPoint;
    this.BaseURL = BaseURL;
  }

  async InsertEntity(entity) {
    this.ADAPTER_OUT(entity);
    return await Post(this.EndPoint, entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async CreateAndCheckout(entity) {
    this.ADAPTER_OUT(entity);
    return await Post(this.EndPoint + '/CreateAndCheckout', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async Checkout(entity) {
    return await Post(this.EndPoint + '/Checkout/' + entity.Id, entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async CancelCheckout(entity) {
    return await Post(this.EndPoint + '/CancelCheckout/' + entity.Id, entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async Checkin(entity) {
    this.ADAPTER_OUT(entity);
    return await Post(this.EndPoint + '/Checkin', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async Finalize(entity) {
    this.ADAPTER_OUT(entity);
    return await Post(this.EndPoint + '/Finalize', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async Unfinalize(entity) {
    this.ADAPTER_OUT(entity);
    return await Post(this.EndPoint + '/Unfinalize', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async MakeRevision(entity) {
    return await Post(this.EndPoint + '/MakeRevision', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async Duplicate(entity) {
    return await Post(this.EndPoint + '/Duplicate', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async CreateInstance(entity) {
    return await Post(this.EndPoint + '/CreateInstance', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async Get(operation) {
    return await Get(this.EndPoint + '/' + operation, this.BaseURL).catch(GeneralError);
  }

  async Post(operation, entity = {}) {
    return await Post(this.EndPoint + '/' + operation, entity, this.BaseURL).catch(GeneralError);
  }

  async GetPaged(limit: number = 0, page: number = 1, params: string = '?') {
    return await Get(this.EndPoint + '/getPaged/' + limit + '/' + page + params, this.BaseURL)
      .then(r => this.UseCommonResponse(r, true))
      .catch(GeneralError);
  }

  async GetSingleWhere(property, value, params = '') {
    if (property && value) {
      return await Get(this.EndPoint + '/GetSingleWhere/' + property + '/' + value + '?' + params, this.BaseURL)
        .then(r => this.UseNudeResponse(r))
        .catch(GeneralError);
    } else if (params.length > 1) {
      return await Get(this.EndPoint + '/GetSingleWhere?' + params, this.BaseURL)
        .then(r => this.UseNudeResponse(r))
        .catch(GeneralError);
    } else {
      return Promise.reject('Invalid params for GetSingleWhere.');
    }
  }

  async LoadEntities(params = '?') {
    return await Get(this.EndPoint + params, this.BaseURL)
      .then(r => this.UseNudeResponse(r))
      .catch(GeneralError);
  }

  async LoadEntity(id) {
    if (id) {
      return await Get(this.EndPoint + '/' + id, this.BaseURL)
        .then(r => this.UseNudeResponse(r))
        .catch(GeneralError);
    } else {
      return Promise.reject('Id not found');
    }
  }

  async Remove(entity) {
    return await Delete(this.EndPoint, entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async RemoveById(id) {
    return await Delete(this.EndPoint + '/' + id, {}, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async Save(entity) {
    if (entity.Id > 0) {
      return await this.UpdateEntity(entity);
    } else {
      return await this.InsertEntity(entity);
    }
  }

  async SendTestEmail(entity) {
    return await Post(this.EndPoint + '/SendTestEmail', entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  async SetProperty(entity, sProperty, Value, qParams) {
    throw 'Not Implemented.';
  }

  async UpdateEntity(entity) {
    this.ADAPTER_OUT(entity);
    return await Put(this.EndPoint, entity, this.BaseURL)
      .then(r => this.UseCommonResponse(r))
      .catch(GeneralError);
  }

  // // LOCAL OPERATIONS
  // getById(id) {
  //   for (let i = 0; i < this.arrAllRecords.length; i++) {
  //     if (id == this.arrAllRecords[i].Id) {
  //       return 0;
  //     }
  //     return null;
  //   }
  // }

  // getAll() {
  //   for (let i = 0; i < this.arrAllRecords.length; i++) {
  //     this.arrAllRecords[i] = this.arrAllRecords[i];
  //   }
  //   return this.arrAllRecords;
  // }

  // getRecursiveBySeedId() {}

  // getRawAll() {
  //   return this.arrAllRecords;
  // }

  // setRawAll(arr) {
  //   this.arrAllRecords = arr;
  // }

  // populateCatalogValues(entity) {
  //   for (let catalog in this.catalogs) {
  //     if (this.catalogs.hasOwnProperty(catalog)) {
  //       entity['' + catalog] = this.catalogs[catalog].getById(entity['' + catalog + 'Key']);
  //     }
  //   }
  // }

  UseCommonResponse = (response: any, returnCommonResponse?: boolean) => {
    //Make sure it is a commonResponse:
    if (!response || !response.hasOwnProperty('ErrorThrown')) throw response;

    //Check for Error:
    if (response.ErrorThrown) throw response;

    //Call Adapter In Hook:
    if (Array.isArray(response.Result)) {
      response.Result.forEach(entity => {
        this.closeRevisions(entity);
        this.ADAPTER_IN(entity);
      });
    } else if (response.Result && typeof response.Result === 'object') {
      this.closeRevisions(response.Result);
      this.ADAPTER_IN(response.Result);
    }

    //Return complete CommonResponse:
    if (returnCommonResponse) return response;

    //Return only data:
    return response.Result;
  };

  UseNudeList = (response, boolWantCommonResponse) => {
    if (response.ErrorThrown) {
      throw response;
    }

    //Call Adapter In Hook:
    if (Array.isArray(response.Result)) {
      response.Result.forEach(entity => {
        this.closeRevisions(entity);
        this.ADAPTER_IN(entity);
      });
    } else if (typeof response.Result === 'object') {
      this.closeRevisions(response.Result);
      this.ADAPTER_IN(response.Result);
    }

    //Return complete CommonResponse:
    if (boolWantCommonResponse) return response;

    //Return only data:
    return response.Result;
  };

  UseNudeResponse = response => {
    if (!response) return response;

    //Call Adapter In Hook:
    if (Array.isArray(response.Result)) {
      response.forEach(entity => {
        this.closeRevisions(entity);
        this.ADAPTER_IN(entity);
      });
    } else if (typeof response === 'object') {
      this.closeRevisions(response);
      this.ADAPTER_IN(response);
    }

    //Return only data:
    return response;
  };

  closeRevisions = entity => {
    entity &&
      entity.Revisions &&
      entity.Revisions.forEach(revision => {
        revision.isOpened = false;
      });
  };

  //Formatters:===================================================================
  formatDate = (date, format = 'M/D/YYYY') => {
    if (date) return moment(date).format(format);
  };

  formatDateMD = (date, format = 'MMMM Do, YYYY') => {
    if (date) return moment(date).format(format);
  };

  formatDateLG = (date, format = 'dddd, MMMM Do, YYYY') => {
    if (date) return moment(date).format(format);
  };

  formatTime = (time, format = 'H:mm a') => {
    if (time) return moment(time).format(format);
  };

  toServerDate = date => {
    var momentDate = moment(date || null);
    if (momentDate.isValid()) {
      momentDate.local();
      return momentDate.format();
    }
    return null;
  };

  toJavascriptDate = from => {
    return from ? new Date(from) : null;
  };

  formatCurrency = (number, decimals = 2) => {
    if (!isNaN(number) && number > 0) {
      return new Intl.NumberFormat('en-US', { maximumFractionDigits: decimals }).format(number);
    }
    return '';
  };

  toJSON = from => {
    if (from == undefined || from == '') return null;
    return JSON.parse(from);
  };

  //Hooks:=======================================================================
  ADAPTER_IN(entity) {
    return entity;
  }

  ADAPTER_OUT(entity) {
    return entity;
  }

  //Accounts:===================================================================
  async GetAccounts(params = '') {
    return await Get('Account?' + params, AppConfig.AuthURL).catch(GeneralError);
  }
}
