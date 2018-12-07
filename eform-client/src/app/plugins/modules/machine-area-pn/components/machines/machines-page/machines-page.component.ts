import {Component, OnInit, ViewChild} from '@angular/core';
import {PageSettingsModel} from 'src/app/common/models/settings';
import {SharedPnService} from '../../../../shared/services';

@Component({
  selector: 'app-machine-area-pn-machines-page',
  templateUrl: './machines-page.component.html',
  styleUrls: ['./machines-page.component.scss']
})
export class MachinesPageComponent implements OnInit {
  @ViewChild('createMachineModal') createMachineModal;
  @ViewChild('editMachineModal') editMachineModal;
  @ViewChild('deleteMachineModal') deleteMachineModal;
  localPageSettings: PageSettingsModel = new PageSettingsModel();
  machines = [
    {
      id: 1,
      name: 'First'
    },
    {
      id: 2,
      name: 'Second'
    }
  ];
  machinesRequestModel = {
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
    this.localPageSettings = this.sharedPnService.getLocalPageSettings
    ('machinesPnSettings', 'Machines').settings;
    this.getAllInitialData();
  }

  updateLocalPageSettings() {
    this.sharedPnService.updateLocalPageSettings
    ('machinesPnSettings', this.localPageSettings, 'Machines');
    this.getAllMachines();
  }

  getAllInitialData() {

  }

  getAllMachines() {

  }

  showEditMachineModal(machine: { id: number; name: string }) {
    this.editMachineModal.show();
  }

  showDeleteMachineModal(machine: { id: number; name: string }) {
    this.deleteMachineModal.show();
  }

  showCreateMachineModal() {
    this.createMachineModal.show();
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
