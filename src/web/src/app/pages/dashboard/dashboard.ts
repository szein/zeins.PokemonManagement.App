import { ChangeDetectorRef, Component, inject, Inject, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { PokemonService } from '../../services/pokemon-service';
import { CollectionStateService } from '../../state/collection-state.service';
import { PokemonStateService } from '../../state/pokemon-state.service';
import { CollectionService } from '../../services/collection-service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class Dashboard implements OnInit {

  private collectionService = inject(CollectionService);
  private collectionStateService = inject(CollectionStateService);
  private pokemonService = inject(PokemonService);
  private pokemonStateService = inject(PokemonStateService);
  private snackBar = inject(MatSnackBar)
  private cdr = inject(ChangeDetectorRef)
  myCollectionCount = 0;
  storeCount = 0;


  ngOnInit(): void {
    this.loadCollectionPokemons();
    this.loadPokemons();
  }

  loadCollectionPokemons() : void {
    this.collectionService
      .getCollectionPokemons().subscribe({
        next: (response) => {          
          if (response[0]) {
            this.collectionStateService.setCollection(response[0].id);
            this.collectionStateService.setCollectionCount(response[0].pokemons.length);
            this.myCollectionCount = this.collectionStateService.collectionCount!;
            this.cdr.markForCheck();
          } else {
            this.myCollectionCount = 0;
          }
        },
        error: (err) => {
          this.snackBar.open('Error! Could not fetch My Default Collection.', 'Close', { duration: 3000 })
          console.error(err);
        }
      });

  }

  loadPokemons(): void {
    this.pokemonService
    .getPokemons(
      0,
      20
    ).subscribe({
      next: (response) => {
        this.pokemonStateService.setStoreCount(response.totalItems);
        this.storeCount = this.pokemonStateService.storeCount!;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.snackBar.open('Error! Could not fetch from Store.', 'Close', { duration: 3000 })
        console.error(err);
      }
    });
  }
}