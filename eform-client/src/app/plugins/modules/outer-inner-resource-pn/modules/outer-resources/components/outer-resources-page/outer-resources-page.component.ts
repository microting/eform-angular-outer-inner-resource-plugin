import {Component, OnDestroy, OnInit} from '@angular/core';
import {
  OuterResourcePnModel,
  OuterResourcesPnModel,
  InnerResourcesPnModel,
  InnerResourcesPnRequestModel,
} from '../../../../models';
import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
} from '../../../../services';
import {OuterResourcesStateService} from '../store';
import {PaginationModel} from 'src/app/common/models';
import {Subscription} from 'rxjs';
import {AutoUnsubscribe} from 'ngx-auto-unsubscribe';
import {Sort} from '@angular/material/sort';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {TranslateService} from '@ngx-translate/core';
import {dialogConfigHelper} from 'src/app/common/helpers';
import {MatDialog} from '@angular/material/dialog';
import {Overlay} from '@angular/cdk/overlay';
import {
  OuterResourceDeleteComponent,
  OuterResourceEditComponent,
  OuterResourceCreateComponent
} from '../';

@AutoUnsubscribe()
@Component({
  selector: 'app-areas-page',
  templateUrl: './outer-resources-page.component.html',
  styleUrls: ['./outer-resources-page.component.scss'],
})
export class OuterResourcesPageComponent implements OnInit, OnDestroy {
  areasModel: OuterResourcesPnModel = new OuterResourcesPnModel();
  mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();

  getAllMachinesSub$: Subscription;
  getAllAreasSub$: Subscription;

  tableHeaders: MtxGridColumn[] = [
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
          click: (record) => this.showEditAreaModal(record),
          tooltip: this.translateService.stream('Edit'),
        },
        {
          type: 'icon',
          color: 'warn',
          icon: 'delete',
          click: (record) => this.showDeleteAreaModal(record),
          tooltip: this.translateService.stream('Delete'),
        },
      ]
    },
  ];
  deleteOuterResourceSub$: Subscription;
  areaUpdatedSub$: Subscription;
  areaCreatedSub$: Subscription;

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    public outerResourcesStateService: OuterResourcesStateService,
    private translateService: TranslateService,
    public dialog: MatDialog,
    private overlay: Overlay,
  ) {
  }

  ngOnInit() {
    this.getAllInitialData();
  }

  ngOnDestroy() {
  }

  getAllInitialData() {
    this.getAllAreas();
    this.getMachinesForMapping();
  }

  getAllAreas() {
    this.getAllAreasSub$ = this.outerResourcesStateService
      .getAllAreas()
      .subscribe((data) => {
        if (data && data.success) {
          this.areasModel = data.model;
        }
      });
  }

  getMachinesForMapping() {
    this.getAllMachinesSub$ = this.machineAreaPnMachinesService
      .getAllMachines(new InnerResourcesPnRequestModel())
      .subscribe((data) => {
        if (data && data.success) {
          this.mappingMachines = data.model;
        }
      });
  }

  showEditAreaModal(area: OuterResourcePnModel) {
    const outerResourceEditModal = this.dialog.open(OuterResourceEditComponent, dialogConfigHelper(this.overlay, {
      areaModel: area,
      mappingMachines: this.mappingMachines,
    }));
    this.areaUpdatedSub$ = outerResourceEditModal.componentInstance.onAreaUpdated.subscribe(_ => this.getAllAreas());
  }

  showDeleteAreaModal(area: OuterResourcePnModel) {
    const outerResourceDeleteModal = this.dialog.open(OuterResourceDeleteComponent, dialogConfigHelper(this.overlay, area));
    this.deleteOuterResourceSub$ = outerResourceDeleteModal.componentInstance.onAreaDeleted.subscribe(_ => this.onAreaDeleted());
  }

  showCreateAreaModal() {
    const outerResourceCreateModal = this.dialog.open(OuterResourceCreateComponent, dialogConfigHelper(this.overlay, this.mappingMachines));
    this.areaCreatedSub$ = outerResourceCreateModal.componentInstance.onAreaCreated.subscribe(_ => this.getAllAreas());
  }

  sortTable(sort: Sort) {
    this.outerResourcesStateService.onSortTable(sort.active);
    this.getAllAreas();
  }

  onAreaDeleted() {
    this.outerResourcesStateService.onDelete();
    this.getAllAreas();
  }

  onPaginationChanged(paginationModel: PaginationModel) {
    this.outerResourcesStateService.updatePagination(paginationModel);
    this.getAllAreas();
  }
}
