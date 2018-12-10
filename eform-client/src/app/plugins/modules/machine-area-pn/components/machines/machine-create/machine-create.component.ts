import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {
  MachineAreaPnAreasService,
  MachineAreaPnMachinesService
} from 'src/app/plugins/modules/machine-area-pn/services';
import {AreaPnCreateModel, AreasPnModel, MachinePnCreateModel, MachinesPnModel} from '../../../models';

@Component({
  selector: 'app-machine-area-pn-machine-create',
  templateUrl: './machine-create.component.html',
  styleUrls: ['./machine-create.component.scss']
})
export class MachineCreateComponent implements OnInit {
  @ViewChild('frame') frame;
  @Input() mappingAreas: AreasPnModel = new AreasPnModel();
  @Output() onNewMachineCreated: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  newMachineModel: MachinePnCreateModel = new MachinePnCreateModel();

  constructor(private machineAreaPnMachinesService: MachineAreaPnMachinesService) { }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  createMachine() {
    this.spinnerStatus = true;
    this.machineAreaPnMachinesService.createMachine(this.newMachineModel).subscribe((data) => {
      this.onNewMachineCreated.emit();
      this.newMachineModel = new MachinePnCreateModel();
      this.spinnerStatus = false;
    });
  }
}
