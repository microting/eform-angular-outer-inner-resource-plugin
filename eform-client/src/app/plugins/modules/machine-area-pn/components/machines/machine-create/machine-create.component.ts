import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';

@Component({
  selector: 'app-machine-area-pn-machine-create',
  templateUrl: './machine-create.component.html',
  styleUrls: ['./machine-create.component.scss']
})
export class MachineCreateComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() onNewMachineCreated: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  newMachineModel: {
    name: ''
  };
  constructor() { }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  createMachine() {

  }
}
