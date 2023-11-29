import {createAction} from '@ngrx/store';

export const updateInnerResourceFilters = createAction(
  '[OuterResource] Update inner resource filters',
  (payload) => ({ payload })
);

export const updateInnerResourcePagination = createAction(
  '[OuterResource] Update inner resource pagination',
  (payload) => ({ payload })
);
