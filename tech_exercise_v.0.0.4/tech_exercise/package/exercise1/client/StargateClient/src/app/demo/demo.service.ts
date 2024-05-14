import { Injectable } from '@angular/core';
import { BehaviorSubject, filter } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DemoService {

  constructor(private http: HttpClient) { }

  // Astronauts
  private _astronautSubject$ = new BehaviorSubject<any[]>([]);
  public readonly astronaut$ = this._astronautSubject$.asObservable();
  getAstronaut(name: string) {
    this.http.get<any[]>("https://localhost:7204/AstronautDuty/" + name)
      .pipe(filter((validObj) => !!validObj))
      .subscribe((returnObj: any[]) => {
        this._astronautSubject$.next(returnObj);
      });
  }
}
