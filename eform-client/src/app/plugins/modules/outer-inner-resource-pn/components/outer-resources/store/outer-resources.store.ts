import { Injectable } from '@angular/core';
import { persistState, Store, StoreConfig } from '@datorama/akita';
import { CommonPaginationState } from 'src/app/common/models';

export interface OuterResourcesState {
  pagination: CommonPaginationState;
  total: number;
}

export function createInitialState(): OuterResourcesState {
  return <OuterResourcesState>{
    pagination: {
      pageSize: 10,
      sort: 'Id',
      isSortDsc: false,
      offset: 0,
    },
    total: 0,
  };
}

const outerResourcesPersistStorage = persistState({
  include: ['outerResources'],
  key: 'outerInnerResourcesPn',
  preStorageUpdate(storeName, state: OuterResourcesState) {
    return {
      pagination: state.pagination,
      // filters: state.filters,
    };
  },
});

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'outerResources', resettable: true })
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
