import {createAction} from '@ngrx/store';

export const updateOuterResourceFilters = createAction(
  '[OuterResource] Update outer resource filters',
  (payload) => ({ payload })
);

export const updateOuterResourcePagination = createAction(
  '[OuterResource] Update outer resource pagination',
  (payload) => ({ payload })
);
