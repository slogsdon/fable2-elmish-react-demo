module FableDemo

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.HMR
open Elmish.React
open Fable.Helpers.React.Props
module R = Fable.Helpers.React
open Fable.Import.Browser

// Let's define our model

type IRoute =
  | Home
  | About
  | NotFound

type Model =
  { count: int
    route: IRoute }

type Msg =
  | FollowLink of IRoute

// Define routes

let routes =
  oneOf
        [ map Home (s "")
          map About (s "about")
          map NotFound (s "404") ]

let urlOfRoute route =
  match route with
  | Home -> "/"
  | About -> "/about"
  | NotFound -> "/404"

let getProperRoute (location: IRoute option) =
  match location with
  | Some r -> Some r
  | None ->
    match (parsePath routes window.location) with
    | Some r -> Some r
    | None -> None

let urlUpdate (result: IRoute option) model =
  match getProperRoute result with
  | Some page ->
      { model with route = page }, Cmd.none

  | None ->
      ( model, Navigation.newUrl "/404" ) // no matching route - go home

// Handle our state initialization and updates

let init (location: IRoute option) =
  urlUpdate location { count = 0; route = Home }

let update model (msg: Msg) =
  match msg with
  | FollowLink route -> { model with route = route }, (Navigation.newUrl <| urlOfRoute route)

// Rendering views with React

let link dispatch (route: IRoute) (text: string) =
  R.a
    [ OnClick (fun e -> e.preventDefault(); dispatch <| FollowLink route)
      Href <| urlOfRoute route ]
    [ R.str text ]

let view dispatch model =
  R.fragment []
      [ R.nav []
          [ link dispatch Home "Home"
            link dispatch About "About" ]

        (match model.route with
         | Home -> R.div [] [ R.str "Home" ]
         | About -> R.div [] [ R.str "About" ]
         | NotFound -> R.div [] [ R.str "Not Found" ])
      ]

// Create the program instance

Program.mkProgram init update view
|> Program.toNavigable (parseHash routes) urlUpdate
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReact "root"
|> Program.run
