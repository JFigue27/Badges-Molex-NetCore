import { CRUDFactory } from '../../core/CRUDFactory';
import AppConfig from '../../core/AppConfig';

export default class CatalogService extends CRUDFactory {
  constructor() {
    super({
      EndPoint: 'Catalog',
      BaseURL: AppConfig.UniversalCatalogsURL
    });
  }

  GetCatalog = async (name: string, wantCommonResponse: boolean = false) => {
    return await this.GetPaged(0, 1, '?CatalogDefinition=' + name).then(r => (wantCommonResponse ? r : r.Result));
  };
}
