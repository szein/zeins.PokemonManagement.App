import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Pokemon } from '../models/pokemon';
import { StoreResponse } from '../models/store-response';

@Injectable({
  providedIn: 'root'
})

export class PokemonService {

  constructor(private http: HttpClient) {}

  getPokemons(
    page: number,
    pageSize: number
){
    return this.http.get<StoreResponse<Pokemon>>(
        `${environment.apiUrl}/api/pokemons`,
        {
            params:{
                page,
                pageSize
            }
        }
    );
  }
}
