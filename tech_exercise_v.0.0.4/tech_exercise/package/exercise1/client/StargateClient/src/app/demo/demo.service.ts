import { Injectable } from '@angular/core';
import { BehaviorSubject, filter } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DemoService {

  constructor(private http: HttpClient) { }

  // Astronauts
  private _astronautsSubject$ = new BehaviorSubject<any[]>([]);
  public readonly astronauts$ = this._astronautsSubject$.asObservable();
  getAllAstronauts() {
    this.http.get<any[]>("https://localhost:7204/AstronautDuty/Reggie")
      .pipe(filter((validObj) => !!validObj))
      .subscribe((returnObj: any[]) => {
        debugger;
        this._astronautsSubject$.next(returnObj);
      });
  }
}
