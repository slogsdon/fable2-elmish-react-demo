module FableDemo

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.HMR
open Elmish.React
open Fable.Helpers.React.Props
module R = Fable.Helpers.React

// Let's define our model

type IRoute =
  | Home
  | About
  | Blog of int
  | Search of string

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
          map Blog (s "blog" </> i32)
          map Search (s "search" </> str) ]

let urlUpdate (result: IRoute option) model =
  Fable.Import.Browser.console.log("location", Fable.Import.Browser.location)
  Fable.Import.Browser.console.log("result", result)
  match result with
  | Some page ->
      { model with route = page }, Cmd.none

  | None ->
      ( model, Navigation.newUrl "/" ) // no matching route - go home

let urlOfRoute route =
  match route with
  | Home -> "/"
  | About -> "/about"
  | _ -> "/"

// Handle our state initialization and updates

let init (location: IRoute option) =
  Fable.Import.Browser.console.log("init location", location)
  Fable.Import.Browser.console.log("init window.location", Fable.Import.Browser.location)

  // let route =
  //   match location with
  //   | Some r -> r
  //   | None ->
  //     match (parsePath routes Fable.Import.Browser.location) with
  //     | Some r -> r
  //     | None -> Home

  { count = 0
    route = Home }, Cmd.none

let update (msg: Msg) model =
  match msg with
  | FollowLink route -> { model with route = route }, (Navigation.newUrl <| urlOfRoute route)

// Rendering views with React

let link dispatch (route: IRoute) (text: string) =
  R.a
    [ OnClick (fun e -> e.preventDefault(); dispatch <| FollowLink route)
      Href <| urlOfRoute route ]
    [ R.str text ]

let view model dispatch =
  R.fragment []
      [ R.nav []
          [ link dispatch Home "Home"
            link dispatch About "About" ]

        (match model.route with
         | Home -> R.div [] [ R.str "Home" ]
         | About -> R.div [] [ R.str "Home" ]
         | _ -> R.div [] [ R.str "Not Found" ])
      ]

// Create the program instance

Program.mkProgram init update view
|> Program.toNavigable (parseHash routes) urlUpdate
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReact "root"
|> Program.run
