import { Component, ElementRef, OnDestroy, OnInit, ViewChild  } from '@angular/core';
import { DemoService } from './demo.service';
import { takeUntil, filter, tap } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-demo',
  templateUrl: './demo.component.html',
  styleUrls: ['./demo.component.scss']
})
export class DemoComponent implements OnInit, OnDestroy {

  astronauts: any[] = [];
  astronautResult: any;
  // astronautApiParam: any = { name: '' };
  constructor(private readonly demoService: DemoService) {}
  
  ngOnInit(){
    this.astronaut$.subscribe();
  }

  onClick(name: string): void{
    this.demoService.getAstronaut(name);
  }

  onSelectionChange(): void{
    alert("Do Something!  A record was just selected.")
  }

  ngOnDestroy(): void {
    
  }

  astronaut$ = this.demoService.astronaut$.pipe(
    tap((astronaut: any[]) => {
      this.astronautResult = JSON.stringify(astronaut);
    }),
  );
}
