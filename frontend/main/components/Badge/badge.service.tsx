import { CRUDFactory } from '../../core/CRUDFactory';

export default class BadgeService extends CRUDFactory {
  constructor() {
    super({
      EndPoint: 'Badge'
    });
  }
  ADAPTER_IN(entity) {
    entity.ConvertedCheckIn = entity.CheckIn ? new Date(entity.CheckIn) : null;
    entity.ConvertedCheckOut = entity.CheckOut ? new Date(entity.CheckOut) : null;
    return entity;
  }

  ADAPTER_OUT(entity) {
    entity.CheckIn = this.toServerDate(entity.ConvertedCheckIn);
    entity.CheckOut = this.toServerDate(entity.ConvertedCheckOut);
  }
}
