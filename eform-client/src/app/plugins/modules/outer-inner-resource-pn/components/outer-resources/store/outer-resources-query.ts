import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import {
  OuterResourcesState,
  OuterResourcesStore,
} from './outer-resources-store';

@Injectable({ providedIn: 'root' })
export class OuterResourcesQuery extends Query<OuterResourcesState> {
  constructor(protected store: OuterResourcesStore) {
    super(store);
  }

  get pageSetting() {
    return this.getValue();
  }

  selectPageSize$ = this.select((state) => state.pagination.pageSize);
  selectIsSortDsc$ = this.select((state) => state.pagination.isSortDsc);
  selectSort$ = this.select((state) => state.pagination.sort);
  selectOffset$ = this.select((state) => state.pagination.offset);
}
