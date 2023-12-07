import {createAction} from '@ngrx/store';

export const updateInnerResourceFilters = createAction(
  '[InnerResource] Update inner resource filters',
  (payload) => ({ payload })
);

export const updateInnerResourcePagination = createAction(
  '[InnerResource] Update inner resource pagination',
  (payload) => ({ payload })
);
