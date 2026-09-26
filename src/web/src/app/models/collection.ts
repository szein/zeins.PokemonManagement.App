import { Pokemon } from "./pokemon";

export interface CollectionModel {
    id: string,
    name: string,
    pokemons: Pokemon[],
    ownerId: string
}
