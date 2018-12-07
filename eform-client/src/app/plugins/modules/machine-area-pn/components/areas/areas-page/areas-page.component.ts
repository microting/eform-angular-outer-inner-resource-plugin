import {Component, OnInit, ViewChild} from '@angular/core';
import {PageSettingsModel} from 'src/app/common/models/settings';
import {SharedPnService} from 'src/app/plugins/modules/shared/services';

@Component({
  selector: 'app-areas-page',
  templateUrl: './areas-page.component.html',
  styleUrls: ['./areas-page.component.scss']
})
export class AreasPageComponent implements OnInit {
  @ViewChild('createAreaModal') createAreaModal;
  @ViewChild('editAreaModal') editAreaModal;
  @ViewChild('deleteAreaModal') deleteAreaModal;
  localPageSettings: PageSettingsModel = new PageSettingsModel();
  areas = [
    {
      id: 1,
      name: 'First'
    },
    {
      id: 2,
      name: 'Second'
    }
  ];
  areasRequestModel = {
    sort: '',
    isSortDsc: false,
    pageSize: 10
  };
  spinnerStatus = false;

  constructor(private sharedPnService: SharedPnService) { }

  ngOnInit() {
    this.getLocalPageSettings();
  }

  getLocalPageSettings() {
    debugger;
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

  }

  getAllAreas() {

  }

  showEditAreaModal(machine: { id: number; name: string }) {
    this.editAreaModal.show();
  }

  showDeleteAreaModal(machine: { id: number; name: string }) {
    this.deleteAreaModal.show();
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

}
