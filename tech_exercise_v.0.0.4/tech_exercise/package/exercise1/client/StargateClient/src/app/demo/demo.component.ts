import { Component, ElementRef, OnDestroy, OnInit, ViewChild  } from '@angular/core';
import { DemoService } from './demo.service';
import { takeUntil, filter, tap } from 'rxjs';

@Component({
  selector: 'app-demo',
  templateUrl: './demo.component.html',
  styleUrls: ['./demo.component.scss']
})
export class DemoComponent implements OnInit, OnDestroy {

  astronauts: any[] = [];
  constructor(private readonly demoService: DemoService) {}
  
  ngOnInit(){
    this.astronaut$.subscribe();
  }

  onClick(): void{
    this.demoService.getAllAstronauts();
  }

  onSelectionChange(): void{
    alert("Do Something!  A record was just selected.")
  }

  ngOnDestroy(): void {
    
  }

  astronaut$ = this.demoService.astronauts$.pipe(
    tap((astronauts: any[]) => {
      debugger;
      this.astronauts = astronauts;
    }),
  );
}
