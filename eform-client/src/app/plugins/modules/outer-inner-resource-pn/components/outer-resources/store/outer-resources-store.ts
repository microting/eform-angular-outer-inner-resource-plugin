import { Injectable } from '@angular/core';
import { persistState, Store, StoreConfig } from '@datorama/akita';
import { CommonPaginationState } from 'src/app/common/models/common-pagination-state';

export interface OuterResourcesState {
  pagination: CommonPaginationState;
}

export function createInitialState(): OuterResourcesState {
  return <OuterResourcesState>{
    pagination: {
      pageSize: 10,
      sort: 'Id',
      isSortDsc: false,
      offset: 0,
    },
  };
}

const outerResourcesPersistStorage = persistState({
  include: ['outerInnerResourcesPnOuterResources'],
  key: 'pluginsStore',
});

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'outerInnerResourcesPnOuterResources', resettable: true })
export class OuterResourcesStore extends Store<OuterResourcesState> {
  constructor() {
    super(createInitialState());
  }
}

export const outerResourcesPersistProvider = {
  provide: 'persistStorage',
  useValue: outerResourcesPersistStorage,
  multi: true,
};
