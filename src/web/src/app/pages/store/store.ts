import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PokemonService } from '../../services/pokemon-service';
import { Pokemon } from '../../models/pokemon';
import { PokemonItem } from '../../components/pokemon-item/pokemon-item';
import { CollectionService } from '../../services/collection-service';
import { CollectionStateService } from '../../state/collection-state.service';
import { PokemonStateService } from '../../state/pokemon-state.service';

@Component({
  imports: [MatPaginatorModule, PokemonItem],
  selector: 'app-store',
  styleUrl: './store.scss',
  templateUrl: './store.html',
})

export class Store implements OnInit {
  private pokemonService = inject(PokemonService)
  private collectionService = inject(CollectionService)
  private collectionStateService = inject(CollectionStateService)
  private pokemonStateService = inject(PokemonStateService)
  private snackBar = inject(MatSnackBar)
  private cdr = inject(ChangeDetectorRef)
  pokemons: Pokemon[] = [];

  pageIndex = 0;
  pageSize = 10;
  totalItems = 0;
  actionButtonText = "Add To Collection";


  ngOnInit(): void {
    this.loadPokemons();
  }

  loadPokemons(): void {
    this.pokemonService
    .getPokemons(
      this.pageIndex + 1,
      this.pageSize
    ).subscribe({
      next: (response) => {
        this.pokemons = response.items;
        this.totalItems = response.totalItems;
        this.pokemonStateService.setStoreCount(this.totalItems);
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.snackBar.open('Error! Could not fetch collection.', 'Close', { duration: 3000 })
        console.error(err);
        
      }
    });
  }

   onActionButtonClick(pokemon: Pokemon){
    this.collectionService.addPokemonToCollection(this.collectionStateService.collectionId!, pokemon).subscribe({
      next: (result) => this.snackBar.open(
        result.alreadyExists ? 'Did not Add Pokemon because is ALREADY in the collection!' : 'Pokemon added to collection.',
        'Close',
        { duration: 3000 }
      ),
      error: () => this.snackBar.open('Could not add Pokemon to collection.', 'Close', { duration: 3000 })
    })
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadPokemons();
}

}
