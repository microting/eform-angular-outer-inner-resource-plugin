import { Injectable } from '@angular/core';
import { persistState, Store, StoreConfig } from '@datorama/akita';
import { CommonPaginationState } from 'src/app/common/models/common-pagination-state';

export interface InnerResourcesState {
  pagination: CommonPaginationState;
}

export function createInitialState(): InnerResourcesState {
  return <InnerResourcesState>{
    pagination: {
      pageSize: 10,
      sort: 'Id',
      isSortDsc: false,
      offset: 0,
    },
  };
}

export const innerResourcesPersistStorage = persistState({
  include: ['outerInnerResourcesPnInnerResources'],
  key: 'pluginsStore',
});

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'outerInnerResourcesPnInnerResources', resettable: true })
export class InnerResourcesStore extends Store<InnerResourcesState> {
  constructor() {
    super(createInitialState());
  }
}

export const innerResourcesPersistProvider = {
  provide: 'persistStorage',
  useValue: innerResourcesPersistStorage,
  multi: true,
};
