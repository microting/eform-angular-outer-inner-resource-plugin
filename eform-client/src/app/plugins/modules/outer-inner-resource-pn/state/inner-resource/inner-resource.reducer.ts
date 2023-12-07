import {CommonPaginationState} from 'src/app/common/models';
import {createReducer, on} from '@ngrx/store';
import {
  updateInnerResourceFilters, updateInnerResourcePagination
} from './inner-resource.actions';

export interface InnerResourcesState {
  pagination: CommonPaginationState;
  total: number;
}

export const initialInnerResourcesState: InnerResourcesState = {
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
  initialInnerResourcesState,
  on(updateInnerResourcePagination, (state, {payload}) => ({
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

export function reducer(state: InnerResourcesState | undefined, action: any) {
  return _reducer(state, action);
}
