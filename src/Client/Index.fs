module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model =
    { Input: string }

type Msg =
    | AddTodo


let init(): Model * Cmd<Msg> =
    let model =
        { Input = "" }    
    let cmd = Cmd.Empty
    model, cmd

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with    
    | _ -> { model with Input = "" }, Cmd.none    

open Fable.React
open Fable.React.Props
open Fulma

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://safe-stack.github.io/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Logo"
            ]
        ]
    ]

let view (model : Model) (dispatch : Msg -> unit) =
    Hero.hero [
        Hero.Color IsPrimary
        Hero.IsFullHeight
        Hero.Props [
            Style [
                Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                BackgroundSize "cover"
            ]
        ]
    ] [
        Hero.head [ ] [            
        ]

        Hero.body [ ] [            
        ]
    ]
