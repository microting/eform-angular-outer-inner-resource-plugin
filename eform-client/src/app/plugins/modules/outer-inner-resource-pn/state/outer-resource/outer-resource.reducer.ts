import {CommonPaginationState} from 'src/app/common/models';
import {createReducer, on} from '@ngrx/store';
import {
  updateOuterResourceFilters, updateOuterResourcePagination
} from './outer-resource.actions';

export interface OuterResourcesState {
  pagination: CommonPaginationState;
  total: number;
}

export const initialOuterResourcesState: OuterResourcesState = {
  pagination: {
    pageSize: 10,
    sort: 'Id',
    isSortDsc: false,
    offset: 0,
    total: 0,
    pageIndex: 0,
  },
  total: 0,
};

export const _reducer = createReducer(
  initialOuterResourcesState,
  on(updateOuterResourceFilters, (state, {payload}) => ({
    ...state,
    filters: payload.filters,
  })),
  on(updateOuterResourcePagination, (state, {payload}) => ({
    ...state,
    pagination: {
      offset: payload.pagination.offset,
      pageSize: payload.pagination.pageSize,
      pageIndex: payload.pagination.pageIndex,
      sort: payload.pagination.sort,
      isSortDsc: payload.pagination.isSortDsc,
      total: payload.pagination.total,
    },
  })),
);


export function reducer(state: OuterResourcesState | undefined, action: any) {
  return _reducer(state, action);
}
