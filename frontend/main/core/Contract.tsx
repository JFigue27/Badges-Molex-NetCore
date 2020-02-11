export enum EntryState {
  Unchanged = 0,
  Upserted = 1,
  Deleted = 2
}

export interface IEntity {
  Id?: number;
  Entry_State?: EntryState;
  Revisions?: Array<any>;
  RevisionMessage?: string;
  AttachmentsFolder?: string;
}

export interface IUserAuth {
  BearerToken: string;
  UserName: string;
}

export interface IAuth {
  user?: IUserAuth;
}

export interface IFormState {
  config: any;
  baseEntity: any;
  isLoading: boolean;
  isDisabled: boolean;
}
