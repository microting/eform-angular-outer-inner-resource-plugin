import {Component, OnInit, ViewChild} from '@angular/core';
import {PageSettingsModel} from 'src/app/common/models/settings';
import {SharedPnService} from 'src/app/plugins/modules/shared/services';
import {OuterResourcePnModel, OuterResourcesPnModel, OuterResourcesPnRequestModel} from '../../../models/area';
import {InnerResourcesPnModel, InnerResourcesPnRequestModel} from '../../../models/machine';
import {OuterInnerResourcePnInnerResourceService, OuterInnerResourcePnOuterResourceService} from '../../../services';

@Component({
  selector: 'app-areas-page',
  templateUrl: './outer-resources-page.component.html',
  styleUrls: ['./outer-resources-page.component.scss']
})
export class OuterResourcesPageComponent implements OnInit {
  @ViewChild('createAreaModal') createAreaModal;
  @ViewChild('editAreaModal') editAreaModal;
  @ViewChild('deleteAreaModal') deleteAreaModal;
  localPageSettings: PageSettingsModel = new PageSettingsModel();
  areasModel: OuterResourcesPnModel = new OuterResourcesPnModel();
  mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();
  areasRequestModel: OuterResourcesPnRequestModel = new OuterResourcesPnRequestModel();
  spinnerStatus = false;

  constructor(private sharedPnService: SharedPnService,
              private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
              private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService) { }

  ngOnInit() {
    this.getLocalPageSettings();
  }

  getLocalPageSettings() {
    this.localPageSettings = this.sharedPnService.getLocalPageSettings
    ('machinesPnSettings', 'Areas').settings;
    this.getAllInitialData();
  }

  updateLocalPageSettings() {
    this.sharedPnService.updateLocalPageSettings
    ('machinesPnSettings', this.localPageSettings, 'Areas');
    this.getAllAreas();
  }

  getAllInitialData() {
    this.getAllAreas();
    this.getMachinesForMapping();
  }

  getAllAreas() {
    this.spinnerStatus = true;
    this.areasRequestModel.isSortDsc = this.localPageSettings.isSortDsc;
    this.areasRequestModel.sort = this.localPageSettings.sort;
    this.areasRequestModel.pageSize = this.localPageSettings.pageSize;
    this.machineAreaPnAreasService.getAllAreas(this.areasRequestModel).subscribe((data) => {
      if (data && data.success) {
        this.areasModel = data.model;
      } this.spinnerStatus = false;
    });
  }

  getMachinesForMapping() {
    this.spinnerStatus = true;
    this.machineAreaPnMachinesService.getAllMachines(new InnerResourcesPnRequestModel()).subscribe((data) => {
      if (data && data.success) {
        this.mappingMachines = data.model;
      } this.spinnerStatus = false;
    });
  }

  showEditAreaModal(area: OuterResourcePnModel) {
    this.editAreaModal.show(area);
  }

  showDeleteAreaModal(area: OuterResourcePnModel) {
    this.deleteAreaModal.show(area);
  }

  showCreateAreaModal() {
    this.createAreaModal.show();
  }

  sortTable(sort: string) {
    if (this.localPageSettings.sort === sort) {
      this.localPageSettings.isSortDsc = !this.localPageSettings.isSortDsc;
    } else {
      this.localPageSettings.isSortDsc = false;
      this.localPageSettings.sort = sort;
    }
    this.updateLocalPageSettings();
  }

  changePage(e: any) {
    if (e || e === 0) {
      this.areasRequestModel.offset = e;
      if (e === 0) {
        this.areasRequestModel.pageIndex = 0;
      } else {
        this.areasRequestModel.pageIndex
          = Math.floor(e / this.areasRequestModel.pageSize);
      }
      this.getAllAreas();
    }
  }

}
