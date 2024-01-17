import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {
  InnerResourcePnModel,
  InnerResourcesPnModel,
  OuterResourcesPnModel,
  OuterResourcesPnRequestModel,
} from '../../../../models';
import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
} from '../../../../services';
import {OuterInnerResourcePnClaims} from '../../../../enums';
import {Subscription} from 'rxjs';
import {AutoUnsubscribe} from 'ngx-auto-unsubscribe';
import {InnerResourcesStateService} from '../store';
import {PaginationModel, TableHeaderElementModel} from 'src/app/common/models';
import {AuthStateService} from 'src/app/common/store';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {TranslateService} from '@ngx-translate/core';
import {MatDialog} from '@angular/material/dialog';
import {Overlay} from '@angular/cdk/overlay';
import {Sort} from '@angular/material/sort';
import {
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourceEditComponent
} from '../';
import {dialogConfigHelper} from 'src/app/common/helpers';
import {Store} from '@ngrx/store';
import {
  selectInnerResourcesPagination,
  selectInnerResourcesPaginationIsSortDsc,
  selectInnerResourcesPaginationSort
} from '../../../../state';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-machines-page',
  templateUrl: './inner-resources-page.component.html',
  styleUrls: ['./inner-resources-page.component.scss'],
})
export class InnerResourcesPageComponent implements OnInit, OnDestroy {
  @ViewChild('createMachineModal', {static: false}) createMachineModal;
  @ViewChild('editMachineModal', {static: false}) editMachineModal;
  @ViewChild('deleteMachineModal', {static: false}) deleteMachineModal;
  machinesModel: InnerResourcesPnModel = new InnerResourcesPnModel();
  mappingAreas: OuterResourcesPnModel = new OuterResourcesPnModel();
  name: string;

  getAllMachinesSub$: Subscription;
  getAllAreasSub$: Subscription;

  tableHeaders: TableHeaderElementModel[] = [
    {name: 'Id', elementId: 'idTableHeader', sortable: true},
    {name: 'Name', elementId: 'nameTableHeader', sortable: true},
    {
      name: 'ExternalId',
      elementId: 'externalIdTableHeader',
      sortable: true,
      visibleName: 'External ID',
    },
    {name: 'Actions', elementId: '', sortable: false},
  ];

  tableHeaders1: MtxGridColumn[] = [
    {header: this.translateService.stream('Id'), field: 'id', sortProp: {id: 'Id'}, sortable: true},
    {header: this.translateService.stream('Name'), sortProp: {id: 'Name'}, field: 'name', sortable: true},
    {header: this.translateService.stream('External ID'), field: 'externalId', sortable: true, sortProp: {id: 'ExternalId'}},
    {
      header: this.translateService.stream('Actions'), field: 'actions',
      type: 'button',
      buttons: [
        {
          type: 'icon',
          color: 'accent',
          icon: 'edit',
          click: (record) => this.showEditMachineModal(record),
          tooltip: this.translateService.stream('Edit'),
        },
        {
          type: 'icon',
          color: 'warn',
          icon: 'delete',
          click: (record) => this.showDeleteMachineModal(record),
          tooltip: this.translateService.stream('Delete'),
        },
      ]
    },
  ];
  deleteInnerResourceSub$: Subscription;
  machineUpdatedSub$: Subscription;
  machineCreatedSub$: Subscription;

  get outerInnerResourcePnClaims() {
    return OuterInnerResourcePnClaims;
  }

  public selectInnerResourcesPagination$ = this.store.select(selectInnerResourcesPagination);
  public selectInnerResourcesPaginationSort$ = this.store.select(selectInnerResourcesPaginationSort);
  public selectInnerResourcesPaginationIsSortDsc$ = this.store.select(selectInnerResourcesPaginationIsSortDsc);

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    public innerResourcesStateService: InnerResourcesStateService,
    public authStateService: AuthStateService,
    private translateService: TranslateService,
    public dialog: MatDialog,
    private overlay: Overlay,
    private store: Store,
  ) {
  }

  ngOnInit() {
    this.getAllInitialData();
  }

  ngOnDestroy() {
  }

  getAllInitialData() {
    this.getAllMachines();
    this.getMappedAreas();
  }

  getAllMachines() {
    this.getAllMachinesSub$ = this.innerResourcesStateService
      .getAllMachines()
      .subscribe((data) => {
        if (data && data.success) {
          this.machinesModel = data.model;
        }
      });
  }

  getMappedAreas() {
    this.getAllAreasSub$ = this.machineAreaPnAreasService
      .getAllAreas(new OuterResourcesPnRequestModel())
      .subscribe((data) => {
        if (data && data.success) {
          this.mappingAreas = data.model;
        }
      });
  }

  showEditMachineModal(machine: InnerResourcePnModel) {
    const innerResourceEditModal = this.dialog.open(InnerResourceEditComponent, dialogConfigHelper(this.overlay, {
      machineModel: machine,
      mappingAreas: this.mappingAreas,
    }));
    this.machineUpdatedSub$ = innerResourceEditModal.componentInstance.onMachineUpdated.subscribe(_ => this.getAllMachines());
  }

  showDeleteMachineModal(machine: InnerResourcePnModel) {
    const innerResourceDeleteModal = this.dialog.open(InnerResourceDeleteComponent, dialogConfigHelper(this.overlay, machine));
    this.deleteInnerResourceSub$ = innerResourceDeleteModal.componentInstance.onMachineDeleted.subscribe(_ => this.onMachineDeleted());
  }

  showCreateMachineModal() {
    const innerResourceCreateModal = this.dialog.open(InnerResourceCreateComponent, dialogConfigHelper(this.overlay, this.mappingAreas));
    this.machineCreatedSub$ = innerResourceCreateModal.componentInstance.onMachineCreated.subscribe(_ => this.getAllMachines());
  }

  sortTable(sort: Sort) {
    this.innerResourcesStateService.onSortTable(sort.active);
    this.getAllMachines();
  }

  // changePage(offset: number) {
  //   this.innerResourcesStateService.changePage(offset);
  //   this.getAllMachines();
  // }

  onMachineDeleted() {
    this.innerResourcesStateService.onDelete();
    this.getAllMachines();
  }

  onPaginationChanged(paginationModel: PaginationModel) {
    this.innerResourcesStateService.updatePagination(paginationModel);
    this.getAllMachines();
  }
}
