// import { Injectable } from '@angular/core';
// import { persistState, Store, StoreConfig } from '@datorama/akita';
// import { CommonPaginationState } from 'src/app/common/models';
//
// export interface InnerResourcesState {
//   pagination: CommonPaginationState;
//   total: number;
// }
//
// export function createInitialState(): InnerResourcesState {
//   return <InnerResourcesState>{
//     pagination: {
//       pageSize: 10,
//       sort: 'Id',
//       isSortDsc: false,
//       offset: 0,
//     },
//     total: 0,
//   };
// }
//
// const innerResourcesPersistStorage = persistState({
//   include: ['innerResources'],
//   key: 'outerInnerResourcesPn',
//   preStorageUpdate(storeName, state: InnerResourcesState) {
//     return {
//       pagination: state.pagination,
//       // filters: state.filters,
//     };
//   },
// });
//
// @Injectable({ providedIn: 'root' })
// @StoreConfig({ name: 'innerResources', resettable: true })
// export class InnerResourcesStore extends Store<InnerResourcesState> {
//   constructor() {
//     super(createInitialState());
//   }
// }
//
// export const innerResourcesPersistProvider = {
//   provide: 'persistStorage',
//   useValue: innerResourcesPersistStorage,
//   multi: true,
// };
