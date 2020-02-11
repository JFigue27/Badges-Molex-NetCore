import React from 'react';
import Router from 'next/router';
import { GlobalContext } from '../components/App/globals-context';
import { CRUDFactory } from './CRUDFactory';
import { IEntity, IFormState, EntryState } from './Contract';

export interface FormProps {
  dialog?: any;
  data?: any;
  onChange?(data: any, field?: string): void;
}

class FormContainer<ExtendedProps> extends React.Component<FormProps & ExtendedProps> {
  auth: any = {};
  service: CRUDFactory;
  criteria: any = undefined;
  dialogPromise: Promise<any>;

  state: IFormState = {
    config: {
      service: null
    },
    isLoading: true,
    baseEntity: {},
    isDisabled: true
  };

  constructor(props: any, config: any) {
    super(props);
    if (config) Object.assign(this.state.config, config);
    this.service = this.state.config.service;
  }

  UNSAFE_componentWillMount() {
    this.auth = this.context.auth;
    const { dialog } = this.props as any;
    if (dialog) dialog.onOk = this.onDialogOk;
  }

  componentDidUpdate() {
    if (!this.auth && this.context.auth && this.context.auth.user) {
      console.log('Refresh after login.');
      if ((this as any).refresh) {
        (this as any).refresh();
      } else {
        this.refreshForm(undefined);
      }
    }
    this.auth = this.context.auth;
  }

  // Service Operations:==========================================================
  load = async (criteria?: any) => {
    this.criteria = criteria;
    return await this.refreshForm(undefined);
  };

  refreshForm = async criteriaParam => {
    let criteria = criteriaParam === undefined ? this.criteria : criteriaParam;
    console.log('Form criteria: ' + criteria);

    //Clear form:
    if (criteria === null) {
      console.log('Form Reset.', criteria);
      const baseEntity = {};
      this.ON_CHANGE(baseEntity);
      this.setState({ baseEntity, isLoading: false });
    }
    //Open by ID
    else if (!isNaN(criteria) && criteria > 0) {
      console.log('Form opened by ID.', criteria);
      //TODO: Catch non-existent record
      return this.service.LoadEntity(criteria).then(baseEntity => {
        baseEntity.isOpened = true;
        this._afterLoad(baseEntity);
        this.ON_CHANGE(baseEntity);
        this.setState({
          isLoading: false,
          baseEntity
        });
      });
    }
    //Open by query parameters
    else if (typeof criteria == 'string') {
      return this.service.GetSingleWhere(null, null, criteria).then(baseEntity => {
        if (!baseEntity) return;

        baseEntity.isOpened = true;
        this._afterLoad(baseEntity);
        this.ON_CHANGE(baseEntity);
        this.setState({ isLoading: false, baseEntity });
      });
    }
    //Create instance
    else if ((criteria instanceof Object || typeof criteria == 'object') && !criteria.hasOwnProperty('Id')) {
      console.log('Form opened by Create Instance.', criteria);
      return this.createInstance(criteria);
    }
    //Open direct object
    else if (criteria instanceof Object || typeof criteria == 'object') {
      console.log('Form opened by Object.', criteria);
      criteria.isOpened = true;
      this.service.ADAPTER_IN(criteria);
      this._afterLoad(criteria);
      this.ON_CHANGE(criteria);
      this.setState({
        baseEntity: criteria,
        isLoading: false
      });
    } else {
      console.log('no form refresh', criteria);
    }
  };

  createInstance = async predefined => {
    // let theArguments = Array.prototype.slice.call(arguments);
    return await this.service.CreateInstance(predefined).then(instance => {
      // theArguments.unshift(instance);
      // this.AFTER_CREATE.apply(this, theArguments);
      instance.Entry_State = EntryState.Upserted;
      this.AFTER_CREATE(instance);
      this.ON_CHANGE(instance);
      this.setState({ baseEntity: instance, isDisabled: false });
    });
  };

  createAndCheckout = async (event, item = {}) => {
    if (event) event.stopPropagation();
    if (confirm(`Please confirm to create a new ${this.service.EndPoint}`)) {
      return await this.service.CreateAndCheckout(item).then(baseEntity => {
        this.AFTER_CREATE_AND_CHECKOUT(baseEntity);
        this.setState({ baseEntity, isDisabled: false });
        this.success('Created and Checked Out.');
        return baseEntity;
      });
    }
  };

  save = (entity = this.state.baseEntity) => {
    return this.BEFORE_SAVE(entity)
      .then(entity => {
        if (entity.api_Attachments) return entity.api_Attachments.uploadFiles();
        else return entity;
      })
      .then(entity => {
        return this.service.Save(entity).then(baseEntity => {
          baseEntity.isOpened = true;
          this.AFTER_SAVE(baseEntity);
          this._afterSave(baseEntity);
          this.ON_CHANGE(baseEntity);
          this.setState({ baseEntity });
          this.success('Saved.');
          return baseEntity;
        });
      })
      .catch(error => {
        console.log(error);
        let sError = JSON.stringify(error);
        this.error(sError);
        // alert(sError);
      });
  };

  onSubmit = event => {
    event.preventDefault();
    this.save();
  };

  onAttachmentsChange = (files, listBind, folderBind, targetFolder) => {
    let { baseEntity } = this.state;
    if (files && listBind) {
      baseEntity[listBind] = [...files];
    }

    if (targetFolder && folderBind) {
      baseEntity[folderBind] = baseEntity[folderBind] || targetFolder;
    }

    this.setState({ baseEntity });

    return baseEntity;
  };

  loadRevision = selectedRevision => {
    this.state.baseEntity.Revisions.forEach(revision => {
      revision.isOpened = false;
    });

    let revision = JSON.parse(selectedRevision.Value);
    revision.Revisions = this.state.baseEntity.Revisions;
    this.service.ADAPTER_IN(revision);
    selectedRevision.isOpened = true;
    this.ON_CHANGE(revision);
    this.setState({ baseEntity: revision });
    this.success('Revision Loaded.');
  };

  // take = async (entity, toUser) => {
  //   return await this.service.Take(entity, toUser).then(() => {
  //     entity.assignedTo = toUser.Value;
  //     entity.AssignationMade = false;
  //     this.success('Assigned.');
  //   });
  // };

  remove = async (event, entity = this.state.baseEntity) => {
    if (event) event.stopPropagation();
    if (confirm(`Are you sure you want to remove it?`)) {
      return await this.service.Remove(entity).then(() => {
        this.AFTER_REMOVE(entity);
        this.success('Removed.');
      });
    }
  };

  duplicate = async () => {
    return await this.service.Duplicate(this.state.baseEntity).then(() => {
      this.success('Duplicated.');
    });
  };

  checkout = async () => {
    return await this.service
      .Checkout(this.state.baseEntity)
      .then(response => {
        this.refreshForm(undefined);
        // this.setState({ isDisabled: false });
        this.success('Checked Out.');
      })
      .catch(() => {
        this.load(this.state.baseEntity.Id);
      });
  };

  cancelCheckout = async () => {
    return await this.service.CancelCheckout(this.state.baseEntity).then(response => {
      this.refreshForm(undefined);
      // this.setState({ isDisabled: true });
      this.success('Cancel Checked Out.');
    });
  };

  checkin = async event => {
    if (event) event.stopPropagation();
    let { baseEntity } = this.state;

    let message = prompt(`Optional message to reference this update.`);
    if (message !== null) {
      baseEntity.Entry_State = EntryState.Upserted;
      baseEntity.RevisionMessage = message;
      return this.BEFORE_CHECKIN(baseEntity)
        .then((entity = baseEntity) => this.BEFORE_SAVE(entity))
        .then(entity => {
          if (entity.api_Attachments) return entity.api_Attachments.uploadFiles();
          return entity;
        })
        .then((entity = baseEntity) => {
          return this.service.Checkin(entity).then(response => {
            this.AFTER_SAVE(response);
            this._afterSave(response);
            this.refreshForm(response).then(() => {
              // this.setState({ isDisabled: true });
              this.success('Checked In.');
            });
          });
        });
    }
  };

  finalize = async () => {
    return await this.service
      .Finalize(this.state.baseEntity)
      .then(response => {
        this.load(response);
        this.success('Finalized.');
      })
      .catch(() => {
        this.load(this.state.baseEntity.Id);
      });
  };

  unfinalize = async () => {
    return await this.service
      .Unfinalize(this.state.baseEntity)
      .then(response => {
        this.load(response);
        this.success('Unfinalized.');
      })
      .catch(() => {
        this.load(this.state.baseEntity.Id);
      });
  };

  onDialogOk = async () => await this.save();

  resolveDialogPromise(result: any) {}
  rejectDialogPromise(result: any) {}

  openDialog = (propId, data: any = true) => {
    let d: any;
    typeof data == 'string' ? (d = data) : (d = { ...data });
    this.setState({ [propId]: d });

    this.dialogPromise = new Promise((resolve, reject) => {
      this.resolveDialogPromise = resolve;
      this.rejectDialogPromise = reject;
    });

    return this.dialogPromise;
  };

  closeDialog = (propId, feedback) => {
    this.setState({
      [propId]: false
    });

    this.ON_DIALOG_CLOSE(propId, feedback);

    if (feedback && feedback != 'cancel') {
      this.resolveDialogPromise({ propId, feedback });
    } else {
      this.rejectDialogPromise({ propId, feedback });
    }
  };

  // Local Operations:============================================================
  handleInputChange: any = (event, field) => {
    let baseEntity = this.state.baseEntity;
    baseEntity[field] = event.target.value;
    this.ON_CHANGE(baseEntity, field);
    this.setState({ baseEntity });
  };

  handleCheckBoxChange: any = (event, field) => {
    let baseEntity = this.state.baseEntity;
    baseEntity[field] = event.target.checked;
    this.ON_CHANGE(baseEntity, field);
    this.setState({ baseEntity });
  };

  handleDateChange: any = (date, field) => {
    let baseEntity = this.state.baseEntity;
    baseEntity[field] = date && date.toDate ? date.toDate() : null;
    this.ON_CHANGE(baseEntity, field);
    this.setState({ baseEntity });
  };

  handleRichText: any = (event, field) => {
    let baseEntity = this.state.baseEntity;
    baseEntity[field] = event.value;
    this.ON_CHANGE(baseEntity, field);
    this.setState({ baseEntity });
  };

  handleChipsChange: any = (value, field) => {
    let baseEntity = this.state.baseEntity;
    baseEntity[field] = value;
    this.ON_CHANGE(baseEntity, field);
    this.setState({ baseEntity });
  };

  handleAutocompleteChange: any = (value, field) => {
    let baseEntity = this.state.baseEntity;
    baseEntity[field] = value ? value.label : null;
    this.ON_CHANGE(baseEntity, field, value);
    this.setState({ baseEntity });
  };

  handleAttachmentsChange: any = (files, listBind, folder) => {
    let { baseEntity } = this.state;
    if (files && listBind) baseEntity[listBind] = [...files];
    if (folder) baseEntity.AttachmentsFolder = baseEntity.AttachmentsFolder || folder;
    this.ON_CHANGE(baseEntity, listBind, folder);
    this.setState({ baseEntity });
    return baseEntity;
  };

  handleAutocomplete: any = (newValue, field) => {
    let baseEntity = { ...this.state.baseEntity };
    baseEntity[field] = newValue;
    this.ON_CHANGE(baseEntity, field, newValue);
    this.setState({ baseEntity });
  };

  getCurrentUser = () => (this.auth && this.auth.user) || {};

  getCheckoutUser = entity => {
    let baseEntity = entity || this.state.baseEntity;
    if (baseEntity && baseEntity.CheckedoutBy) return baseEntity.CheckedoutBy;
    return '';
  };

  isCheckedOutByCurrentUser = entity => {
    let user = this.getCurrentUser();
    let checkedOutBy = this.getCheckoutUser(entity);
    if (user.UserName && checkedOutBy && checkedOutBy.toLowerCase() == user.UserName.toLowerCase()) {
      return true;
    }
    return false;
  };

  _afterLoad = baseEntity => {
    console.log('_afrerLoad');
    if (this.isCheckedOutByCurrentUser(baseEntity)) {
      this.setState({ isDisabled: false });
    } else {
      this.setState({ isDisabled: true });
    }

    this.AFTER_LOAD(baseEntity);
  };

  clear = () => {
    const baseEntity = {};
    this.ON_CHANGE(baseEntity);
    this.setState({ baseEntity });
  };

  makeQueryParameters = fromObject => {
    let result = '?';
    if (fromObject instanceof Object || typeof fromObject == 'object')
      Object.getOwnPropertyNames(fromObject).forEach(prop => {
        result += `&${prop}=${fromObject[prop]}`;
      });

    return result;
  };

  navigateTo(href) {
    return Router.push(href);
  }

  getParameterByName(name, url = '') {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)', 'i'),
      results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
  }

  success = (message, autoHideDuration = 700) => {
    (this.props as any).enqueueSnackbar(message, { variant: 'success', autoHideDuration });
  };
  error = (message, autoHideDuration = 3000) => {
    (this.props as any).enqueueSnackbar(message, { variant: 'error', autoHideDuration });
  };
  info = (message, autoHideDuration = 700) => {
    (this.props as any).enqueueSnackbar(message, { variant: 'info', autoHideDuration });
  };
  message = (message, autoHideDuration = 700) => {
    (this.props as any).enqueueSnackbar(message, { autoHideDuration });
  };

  //Formatters:===================================================================
  formatDate = (date, format?) => {
    if (this.service) return this.service.formatDate(date, format);
  };
  formatDateMD = (date, format?) => {
    if (this.service) return this.service.formatDateMD(date, format);
  };
  formatDateLG = (date, format?) => {
    if (this.service) return this.service.formatDateLG(date, format);
  };
  formatTime = (time, format?) => {
    if (this.service) return this.service.formatTime(time, format);
  };
  formatCurrency = (number, decimals?) => {
    if (this.service) return this.service.formatCurrency(number, decimals);
  };
  stringify = any => {
    return JSON.stringify(any, null, 3);
  };

  //Local Storage:================================================================
  storageSet = (key, value) => {
    localStorage.setItem(key, JSON.stringify(value));
  };

  storageGet = key => {
    let item = localStorage.getItem(key);
    if (item) return JSON.parse(item);
    return null;
  };

  // Events:======================================================================
  on_input_change = item => {
    item.Entry_State = EntryState.Upserted;
  };

  // Hooks:=======================================================================
  AFTER_LOAD = entity => {};

  AFTER_CREATE = instance => {};

  AFTER_CREATE_AND_CHECKOUT = entity => {};

  BEFORE_SAVE = async entity => entity;

  _afterSave = entity => {
    const { dialog } = this.props as any;
    if (dialog) dialog.close('ok');
  };
  AFTER_SAVE = entity => {};

  AFTER_REMOVE = entity => {};

  BEFORE_CHECKIN = async (entity: IEntity) => entity;

  ON_CHANGE = (data: any, field?: string, value?: any) => {
    (this.props as any).onChange && (this.props as any).onChange(data, field);
  };

  ON_DIALOG_CLOSE = (dialogId, feedback) => {
    if ((this as any).refresh && feedback != 'cancel') (this as any).refresh();
    console.log(dialogId, feedback);
  };

  render(): JSX.Element {
    return <></>;
  }
}

FormContainer.contextType = GlobalContext;

export default FormContainer;
