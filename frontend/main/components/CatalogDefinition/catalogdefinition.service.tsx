import { CRUDFactory } from '../../core/CRUDFactory';

///start:slot:dependencies<<<///end:slot:dependencies<<<

// const utilsService = new UtilsService();

export default class CatalogDefinitionService extends CRUDFactory {
  constructor() {
    super({
      EndPoint: 'CatalogDefinition'
    });
  }
  ADAPTER_IN(entity) {
    ///start:slot:adapterIn<<<
    if (entity.Fields) {
      entity.ConvertedFields = entity.Fields.filter(f => !f.FieldType || !f.FieldType.startsWith('Relationship'));
      entity.ConvertedRelationships = entity.Fields.filter(f => f.FieldType && f.FieldType.startsWith('Relationship'));
    }

    if (entity.ConvertedRelationships) {
      entity.ConvertedRelationships.forEach(relationship => {
        relationship.ForeignValue = '';
        if (relationship.Foreign) {
          relationship.ForeignValue = relationship.Foreign.Name;
        }
      });
    }
    ///end:slot:adapterIn<<<
    return entity;
  }

  ADAPTER_OUT(entity) {
    ///start:slot:adapterOut<<<
    if (entity.Fields) {
      entity.Fields = entity.Fields.filter(d => {
        return Object.getOwnPropertyNames(d).some(prop => {
          if (prop == 'edited') return false;
          return d[prop];
        });
      });
      entity.Fields = entity.Fields;
    }
    ///end:slot:adapterOut<<<
  }

  ///start:slot:service<<<///end:slot:service<<<
}
