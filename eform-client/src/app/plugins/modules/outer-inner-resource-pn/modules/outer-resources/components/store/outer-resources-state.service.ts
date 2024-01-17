import {Injectable} from '@angular/core';
import {tap} from 'rxjs';
import {
  CommonPaginationState,
  PaginationModel,
} from 'src/app/common/models';
import {updateTableSort} from 'src/app/common/helpers';
import {OuterInnerResourcePnOuterResourceService} from '../../../../services';
import {Store} from '@ngrx/store';
import {
  selectOuterResourcesPagination,
  updateOuterResourcePagination,
  updateOuterResourceTotal,
} from '../../../../state';

@Injectable({providedIn: 'root'})
export class OuterResourcesStateService {
  private selectOuterResourcesPagination$ = this.store.select(selectOuterResourcesPagination);
  currentPagination: CommonPaginationState;

  constructor(
    private store: Store,
    private service: OuterInnerResourcePnOuterResourceService,
  ) {
    this.selectOuterResourcesPagination$.subscribe(x => this.currentPagination = x);
  }

  getAllAreas() {
    return this.service.getAllAreas({
      ...this.currentPagination
    }).pipe(
      tap((response) => {
        if (response && response.success && response.model) {
          this.store.dispatch(updateOuterResourceTotal(response.model.total));
        }
      })
    );
  }

  onDelete() {
    this.store.dispatch(updateOuterResourceTotal(this.currentPagination.total - 1));
  }

  onSortTable(sort: string) {
    const localPageSettings = updateTableSort(
      sort,
      this.currentPagination.sort,
      this.currentPagination.isSortDsc
    );
    this.store.dispatch(updateOuterResourcePagination({
      ...this.currentPagination,
      ...localPageSettings,
    }));
  }

  updatePagination(pagination: PaginationModel) {
    this.store.dispatch(updateOuterResourcePagination({
      ...this.currentPagination,
      pageSize: pagination.pageSize,
      offset: pagination.offset,
    }));
  }
}
