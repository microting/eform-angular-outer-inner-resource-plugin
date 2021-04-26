import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import {
  InnerResourcesState,
  InnerResourcesStore,
} from './inner-resources-store';

@Injectable({ providedIn: 'root' })
export class InnerResourcesQuery extends Query<InnerResourcesState> {
  constructor(protected store: InnerResourcesStore) {
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
