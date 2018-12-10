import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {} from 'src/app/plugins/modules/machine-area-pn/models/area';
import {AreaPnCreateModel, MachinesPnModel} from '../../../models';
import {MachineAreaPnAreasService} from '../../../services';

@Component({
  selector: 'app-machine-area-pn-area-create',
  templateUrl: './area-create.component.html',
  styleUrls: ['./area-create.component.scss']
})
export class AreaCreateComponent implements OnInit {
  @ViewChild('frame') frame;
  @Input() mappingMachines: MachinesPnModel = new MachinesPnModel();
  @Output() onNewAreaCreated: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  newAreaModel: AreaPnCreateModel = new AreaPnCreateModel();

  constructor(private machineAreaPnAreasService: MachineAreaPnAreasService) { }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  createArea() {
    this.spinnerStatus = true;
    this.machineAreaPnAreasService.createArea(this.newAreaModel).subscribe((data) => {
      this.onNewAreaCreated.emit();
      this.newAreaModel = new AreaPnCreateModel();
      this.spinnerStatus = false;
    });
  }
}
