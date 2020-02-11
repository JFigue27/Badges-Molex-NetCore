import { CRUDFactory } from '../../core/CRUDFactory';

export default class CatalogService extends CRUDFactory {
  constructor() {
    super({
      EndPoint: 'Catalog'
    });
  }

  GetCatalog = async (name: string, wantCommonResponse: boolean = false) => {
    return await this.GetPaged(0, 1, '?CatalogDefinition=' + name).then(r => (wantCommonResponse ? r : r.Result));
  };
}
