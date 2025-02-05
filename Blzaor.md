# Blzaor

## 项目目录

* *`App.razor`* 是应用的根组件
* *`Routes.razor`* 配置 `Blazor` 路由器
* *`Components/Pages`* 目录包含应用的一些网页
* 导航菜单在 `NavMenu` 组件中定义

## Razor

* `Razor` 指令始终以 `@` 字符开头

* 使用 `Razor` 语法创作 `Blazor` 组件，`Razor` 是一种方便使用的 `HTML`、`CSS` 和 `C#` 的混合体

* `Razor` 文件编译为封装组件呈现逻辑的 `C#` 类

* `@page` 指令指定此页面的路由：`@page "/"`

* `@code` 块用于将 `C#` 类成员（字段、属性和方法）添加到组件。 可以使用 `@code` 跟踪组件状态、添加组件参数、实现组件生命周期事件以及定义事件处理程序

* 通过使用 `@rendermode` 指令应用交互式呈现模式，使组件具有交互性

* 在 `Razor` 中呈现 `C#` 表达式的值，可以使用前导 `@` 字符

  * 使用 `parens` 来明确表达式的开始和结束：`@(currentCount)`

    ```c#
    <p role="status">Current count: @(currentCount)</p>
    ```

    

* 用 `C# Lambda` 表达式内联定义事件处理程序

  ```c#
  <button class="btn btn-primary" @onclick="() => currentCount++">Click me</button>
  ```

* 双向数据绑定：可以使用 `@bind` 属性将 `UI` 元素绑定到代码中的特定值

  ```c#
  <input @bind="text" />
  <button @onclick="() => text = string.Empty">Clear</button>
  <p>@text</p>
  
  @code {
      string text = "";
  }
  ```

*  `@inject` 指令将服务添加到当前组件并启动其实例，在指令中，指定服务类的名称，后跟要用于此组件中的服务实例的名称

* 当组件初始化完成且收到初始参数时，在呈现页面并显示给用户之前，将触发此事件

  ```c#
   protected override async Task OnInitializedAsync()
   {
   
   }
  ```

  

## Blzaor组件

* `Blazor` 组件的名称必须以`大写字符开头`

* `Blazor` 组件封装了其呈现和 `UI` 事件处理逻辑

  * 在编译时，每个 `Razor` 组件都内置于 `C#` 类中。 类包括常见 `UI` 元素，如状态、呈现逻辑、生命周期方法和事件处理程序

* 组件参数通过向具有 `[Parameter]` 属性的组件添加公共 `C#` 属性来定义

* 若要处理来自组件的 `UI` 事件并使用数据绑定，该组件必须是交互式的

  ```c#
  @rendermode InteractiveServer
  
  或
  
  <Counter @rendermode="InteractiveServer" />
  ```

* 交互式服务器呈现通过与浏览器的 `WebSocket` 连接处理来自服务器的 `UI` 事件。 `Blazor` 通过此连接将 `UI` 事件发送到服务器，以便应用的组件可以处理它们。 然后，`Blazor` 会使用呈现的更新来更新浏览器 `DOM`

## 代码托管模型

* `Blazor Server`：在此模型中，应用是在 `ASP.NET Core` 应用的 `Web` 服务器上执行的。 客户端上的 `UI` 更新、事件和 `JavaScript` 调用通过客户端与服务器之间的 `SignalR` 连接发送。
* `Blazor WebAssembly`：在此模型中，`Blazor` 应用、其依赖项以及 `.NET` 运行时均在浏览器中下载并运行

## 组件之间共享信息

* 使用组件参数或级联参数将值从父组件发送到子组件。 `AppState` 模式是另一种可用于存储值并从应用程序中的任何组件访问这些值的方法

### 使用组件参数与其他组件共享信息

* 在子组件上定义这些参数，然后在父组件中设置其值

* 子组件中定义组件参数。 将其定义为 C# 公共属性，并使用 `[Parameter]` 特性对其进行修饰

  ```c#
  <h2>New Pizza: @PizzaName</h2>
  
  <p>@PizzaDescription</p>
  
  @code {
      [Parameter]
      public string PizzaName { get; set; }
      
      
      // 如果父组件不传递值，将呈现此值。 否则，它将被从父组件传递的值替代
      [Parameter]
      public string PizzaDescription { get; set; } = "The best pizza you've ever tasted."
  }
  ```

* 还可以将项目中的自定义类用作组件参数

  ```c#
  <h2>New Topping: @Topping.Name</h2>
  
  <p>Ingredients: @Topping.Ingredients</p>
  
  @code {
      [Parameter]
      public PizzaTopping Topping { get; set; }
  }
  ```

* 在父组件中，使用子组件标记的属性设置参数值。 直接设置简单组件。 借助基于自定义类的参数，使用内联 C# 代码创建该类的新实例并设置其值

  ```c#
  @page "/pizzas-toppings"
  
  <h1>Our Latest Pizzas and Topping</h1>
  
  <Pizza PizzaName="Hawaiian" PizzaDescription="The one with pineapple" />
  
  <PizzaTopping Topping="@(new PizzaTopping() { Name = "Chilli Sauce", Ingredients = "Three kinds of chilli." })" />
  ```

### 使用级联参数共享信息

* 在组件中设置级联参数的值时，其值将自动提供给所有子组件

* 在父组件中，使用 `<CascadingValue>` 标记指定将级联到所有子组件的信息。 此标记作为内置的 `Blazor` 组件实现。 在该标记内呈现的任何组件都能够访问该值

  ```c#
  @page "/specialoffers"
  
  <h1>Special Offers</h1>
  
  <CascadingValue Name="DealName" Value="Throwback Thursday">
      <!-- 在此呈现的任何后代组件都可以访问级联值。 -->
  </CascadingValue>
  ```

* 在子组件中，可以通过使用组件成员并使用 `[CascadingParameter]` 特性对其进行修饰来访问级联值

  ```c#
  <h2>Deal: @DealName</h2>
  
  @code {
      [CascadingParameter(Name="DealName")]
      private string DealName { get; set; }
  }
  ```

* 至于组件参数，如果有更复杂的需求，可以将对象作为级联参数传递

* 在上面的示例中，级联值由父级中的 `Name` 属性标识，与 `[CascadingParameter]` 属性中的 `Name` 值匹配。 可以选择省略这些名称，在这种情况下，属性将按类型匹配。 只有一个该类型的参数时，可省略名称。 如果要实现两个不同字符串值的级联，则必须使用参数名称以避免任何歧义

### 使用 AppState 共享信息

*  创建一个定义要存储的属性的类，并将其注册为作用域服务。 在要设置或使用 `AppState` 值的任何组件中，注入该服务，然后可以访问其属性。 不同于组件参数和级联参数，`AppState` 中的值可用于应用程序中的所有组件，即使这些组件不是存储该值的组件的子组件也是如此

  ```c#
  public class PizzaSalesState
  {
      public int PizzasSoldToday { get; set; }
  }
  
  // Add the AppState class
  builder.Services.AddScoped<PizzaSalesState>();
  
  
  @page "/"
  @inject PizzaSalesState SalesState
  
  <h1>Welcome to Blazing Pizzas</h1>
  
  <p>Today, we've sold this many pizzas: @SalesState.PizzasSoldToday</p>
  
  <button @onclick="IncrementSales">Buy a Pizza</button>
  
  @code {
      private void IncrementSales()
      {
          SalesState.PizzasSoldToday++;
      }
  }
  ```

## 数据绑定

### 将元素绑定到特定事件

* `@bind` 指令非常智能，并且了解它所使用的控件。 例如，在将值绑定到文本框 `<input>` 时，它将绑定 `value` 属性。 HTML 复选框 `<input>` 具有 `checked` 属性，而不是 `value` 属性。 `@bind` 属性将自动改用此 `checked` 属性。 默认情况下，该控件绑定到 DOM `onchange` 事件
* 使用 `@bind-value` 和 `@bind-value:event` 指令，绑定到指定事件

### 设置绑定值的格式

* 使用 `@bind:format` 指令指定单个日期格式字符串

  ```c#
  @page "/ukbirthdaypizza"
  
  <h1>Order a pizza for your birthday!</h1>
  
  <p>
      Enter your birth date:
      <input @bind="birthdate" @bind:format="dd-MM-yyyy" />
  </p>
  
  @code {
      private DateTime birthdate { get; set; } = new(2000, 1, 1);
  }
  ```

* 编写时，仅支持日期值格式字符串。 将来可能会添加货币格式、数字格式以及其他格式。 若要查看有关绑定格式的最新信息，请参阅 `Blazor` 文档中的[格式字符串](https://learn.microsoft.com/zh-cn/aspnet/core/blazor/components/data-binding#format-strings-1)

* 可以编写 C# 代码来设置绑定值的格式，作为使用 `@bind:format` 指令的一种替代方法。 在成员定义中使用 `get` 和 `set` 访问器

  ```c#
  private string ApprovalRating
      {
          get => approvalRating.ToString("0.000", culture);
          set
          {
              if (Decimal.TryParse(value, style, culture, out var number))
              {
                  approvalRating = Math.Round(number, 3);
              }
          }
      }
  ```


## 路由组件

* `Blazor` 使用名为 `Router` 组件的专用组件路由请求，该组件在 `App.razor` 中的配置如下

  ```c#
  <Router AppAssembly="@typeof(Program).Assembly">
      <Found Context="routeData">
          <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
      </Found>
      <NotFound>
          <p>Sorry, we haven't found any pizzas here.</p>
      </NotFound>
  </Router>
  ```

* 应用启动时，`Blazor` 会检查 `AppAssembly` 属性，以了解它应扫描哪个程序集。 它会扫描该程序集，以寻找具有 `RouteAttribute` 的组件。 `Blazor` 使用这些值编译 `RouteData` 对象，该对象指定如何将请求路由到组件。 编写应用代码时，可以在每个组件中使用 `@page` 指令来修复 `RouteAttribute`

* `<Found>` 标记指定在运行时处理路由的组件：`RouteView` 组件。 此组件接收 `RouteData` 对象以及来自 `URI` 或查询字符串的任何参数。 然后，它呈现指定的组件及其布局。 可以使用 `<Found>` 标记来指定默认布局，当所选组件未通过 `@layout` 指令指定布局时，将使用该布局

* `<NotFound>` 标记指定在不存在匹配路由时返回给用户的内容。 上面的示例返回单个 `<p>` 段落，但你可以呈现更复杂的 HTML。 例如，可能包括指向主页或站点管理员联系人页面的链接

* 在 `Blazor` 组件中，`@page` 指令指定该组件应直接处理请求。 可以在 `@page` 指令中指定 `RouteAttribute`，方法是以字符串的形式传递它。 例如，使用此属性指定页面处理对 /Pizzas 路由的请求：`@page "pizzas"`

* 如果要指定到组件的多个路由，请使用两个或更多 `@page` 指令，如本例所示

  ```c#
  @page "/Pizzas"
  @page "/CustomPizzas"
  ```

### NavigationManager 导航

* 可以使用 `NavigationManager` 对象来获取所有这些值。 必须将对象注入组件，然后才能访问其属性

  * 当前完整 URI，例如 `http://www.contoso.com/pizzas/margherita?extratopping=pineapple`。
  * 基本 URI，例如 `http://www.contoso.com/`。
  * 基本相对路径，例如 `pizzas/margherita`。
  * 查询字符串，例如 `?extratopping=pineapple`。

  ```c#
  @page "/pizzas"
  @inject NavigationManager NavManager
  
  <h1>Buy a Pizza</h1>
  
  <p>I want to order a: @PizzaName</p>
  
  <a href=@HomePageURI>Home Page</a>
  
  @code {
      [Parameter]
      public string PizzaName { get; set; }
      
      public string HomePageURI { get; set; }
      
      protected override void OnInitialized()
      {
          HomePageURI = NavManager.BaseUri;
      }
  }
  ```

* 若要访问查询字符串，必须分析完整 URI。 若要执行此分析，请使用 `Microsoft.AspNetCore.WebUtilities` 程序集中的 `QueryHelpers` 类：

  ```c#
  @page "/pizzas"
  @using Microsoft.AspNetCore.WebUtilities
  @inject NavigationManager NavManager
  
  <h1>Buy a Pizza</h1>
  
  <p>I want to order a: @PizzaName</p>
  
  <p>I want to add this topping: @ToppingName</p>
  
  @code {
      [Parameter]
      public string PizzaName { get; set; }
      
      private string ToppingName { get; set; }
      
      protected override void OnInitialized()
      {
          var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
          if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("extratopping", out var extraTopping))
          {
              ToppingName = System.Convert.ToString(extraTopping);
          }
      }
  }
  ```

* 通过调用 `NavigationManager.NavigateTo()` 方法，使用 `NavigationManager` 对象将用户转交给代码中的另一个组件：`NavManager.NavigateTo("buypizza");`

* 传递给 `NavigateTo()` 方法的字符串是要发送给用户的绝对或相对 URI。 请确保已在该地址设置组件。 对于上述代码，具有 `@page "/buypizza"` 指令的组件将处理此路由

### NavLink 组件

* 在 `Blazor` 中，使用 `NavLink` 组件来呈现 `<a>` 标记，因为它在链接的 `href` 属性与当前 `URL` 匹配时将切换 `active` `CSS` 类
* `NavLink` 组件中的 `Match` 属性用于管理突出显示链接的时间。 有两个选项
  * `NavLinkMatch.All`：使用此值时，只有在链接的 `href` 与当前 URL 完全匹配时，该链接才突出显示为活动链接
  * `NavLinkMatch.Prefix`：使用此值时，当链接的 `href` 与当前 URL 的第一部分匹配时，该链接就突出显示为活动链接。 例如，假设你拥有链接 `<NavLink href="pizzas" Match="NavLinkMatch.Prefix">`。 当前 URL 为 `http://www.contoso.com/pizzas` 及该 URL 中的任意位置（例如 `http://www.contoso.com/pizzas/formaggio`）时，此链接将突出显示为活动链接。 此行为可帮助用户了解自己当前正在查看网站的哪一部分

### 路由参数

* http://www.contoso.com/favoritepizza/hawaiian 若要获取 `hawaiian`，可以将其声明为路由参数

* 使用 `@page` 指令指定将作为路由参数传递给组件的 URI 部分

  ```c#
  @page "/FavoritePizzas/{favorite}"
  
  <h1>Choose a Pizza</h1>
  
  <p>Your favorite pizza is: @Favorite</p>
  
  @code {
      [Parameter]
      public string Favorite { get; set; }
  }
  ```

* 组件参数是从父组件发送到子组件的值。 在父组件中，以子组件标记属性的形式指定组件参数值。 路由参数的指定方式不同。 它们作为 URI 的一部分指定。 Blazor 路由器在后台截获这些值，并将其作为组件值发送到组件，因此你可以以相同的方式访问它们。 路由参数不区分大小写，将转发给同名的组件参数。

### 可选路由参数

* 要使路由参数可选，请使用问号

  ```c#
  @page "/FavoritePizzas/{favorite?}"
  
  <h1>Choose a Pizza</h1>
  
  <p>Your favorite pizza is: @Favorite</p>
  
  @code {
      [Parameter]
      public string Favorite { get; set; }
      
      protected override void OnInitialized()
      {
          Favorite ??= "Fiorentina";
      }
  }
  ```

### 路由约束

* 路由参数的特定类型称为“路由约束”

  ```c#
  @page "/FavoritePizza/{preferredsize:int}"
  
  <h1>Choose a Pizza</h1>
  
  <p>Your favorite pizza size is: @FavoriteSize inches.</p>
  
  @code {
      [Parameter]
      public int FavoriteSize { get; set; }
  }
  ```

* 可以在约束中使用其他这些类型：

  | 约束     | 示例                 | 匹配项示例                                                   |
  | :------- | :------------------- | :----------------------------------------------------------- |
  | bool     | {vegan:bool}         | `http://www.contoso.com/pizzas/true`                         |
  | datetime | {birthdate:datetime} | `http://www.contoso.com/customers/1995-12-12`                |
  | decimal  | {maxprice:decimal}   | `http://www.contoso.com/pizzas/15.00`                        |
  | double   | {weight:double}      | `http://www.contoso.com/pizzas/1.234`                        |
  | float    | {weight:float}       | `http://www.contoso.com/pizzas/1.564`                        |
  | guid     | {pizza id:guid}      | `http://www.contoso.com/pizzas/CD2C1638-1638-72D5-1638-DEADBEEF1638` |
  | long     | {totals ales:long}   | `http://www.contoso.com/pizzas/568192454`                    |

### 设置捕获全部路由参数

* 假设用户尝试通过请求 URI `http://www.contoso.com/favoritepizza/margherita/hawaiian` 来指定两个喜好。 页面将显示消息“你喜欢的披萨是：玛格丽特”，并忽略子文件夹“夏威夷”。 可以使用捕获全部路由参数来更改此行为，该参数捕获跨多个 URI 文件夹边界（正斜杠）的路径。 将星号 (`*`) 作为路由参数名称前缀，使路由参数捕获全部

  ```c#
  @page "/FavoritePizza/{*favorites}"
  
  <h1>Choose a Pizza</h1>
  
  <p>Your favorite pizzas are: @Favorites</p>
  
  @code {
      [Parameter]
      public string Favorites { get; set; }
  }
  ```

* 使用相同的请求 URI，页面现在会显示消息“你喜欢的披萨是：玛格丽特/夏威夷风味”

## Blzaor布局

* 使用布局组件来简化和重用通用 UI 元素

* Blazor 布局是特定类型的组件，因此编写 Blazor 布局与编写其他组件以在应用中呈现 UI 类似

* 文件通常存储在应用的“`Shared`”文件夹中，但你可以选择将其存储在使用它的组件可访问的任何位置

* Blazor 布局组件有两个独特的要求

  * 必须继承 `LayoutComponentBase` 类
  * 必须在要呈现引用的组件内容的位置包含 `@Body` 指令

  ```c#
  @inherits LayoutComponentBase
  
  <header>
      <h1>Blazing Pizza</h1>
  </header>
  
  <nav>
      <a href="Pizzas">Browse Pizzas</a>
      <a href="Toppings">Browse Extra Toppings</a>
      <a href="FavoritePizzas">Tell us your favorite</a>
      <a href="Orders">Track Your Order</a>
  </nav>
  
  @Body
  
  <footer>
      @new MarkdownString(TrademarkMessage)
  </footer>
  
  @code {
      public string TrademarkMessage { get; set; } = "All content is &copy; Blazing Pizzas 2021";
  }
  ```

* 布局组件不包括 `@page` 指令，因为它们不直接处理请求，不应为它们创建路由

* 如果要将模板应用于文件夹中的所有 Blazor 组件，可以使用 `_Imports.razor` 文件作为快捷方式

* Blazor 编译器找到此文件时，会自动在文件夹中的所有组件中包含其指令。 使用此方法，无需再将 `@layout` 指令添加到每个组件，适用于 `_Imports.razor` 文件所在文件夹及其所有子文件夹中的组件

* 请勿向项目的根文件夹中的 `_Imports.razor` 文件添加 `@layout` 指令，因为这会导致布局的无限循环

* 如果要将默认布局应用于 Web 应用的所有文件夹中的所有组件，可以在 `App.razor` 组件中执行此操作

  ```c#
  <Router AppAssembly="@typeof(Program).Assembly">
      <Found Context="routeData">
          <RouteView RouteData="@routeData" DefaultLayout="@typeof(BlazingPizzasMainLayout)" />
      </Found>
      <NotFound>
          <p>Sorry, there's nothing at this address.</p>
      </NotFound>
  </Router>
  ```

* 在各自 `@layout` 指令或 `_Imports.razor` 文件中指定了布局的组件将覆盖此默认布局设置

## 事件处理程序

* 使用 `@ref` 属性指定元素引用，并在代码中创建一个同名的 C# 对象

  ```c#
  <button class="btn btn-primary" @onclick="ChangeFocus">Click me to change focus</button>
  <input @ref=InputField @onfocus="HandleFocus" value="@data"/>
  
  @code {
      private ElementReference InputField;
      private string data;
  
      private async Task ChangeFocus()
      {
          await InputField.FocusAsync();
      }
  
      private async Task HandleFocus()
      {
          data = "Received focus";
      }
  ```

### 编写内联事件处理程序

* 如果你有一个不需要在页面或组件中的其他位置重用的简单事件处理程序，则 Lambda 表达式非常有用

  ```c#
  @page "/counter"
  
  <h1>Counter</h1>
  
  <p>Current count: @currentCount</p>
  
  <button class="btn btn-primary" @onclick="() => currentCount++">Click me</button>
  
  @code {
      private int currentCount = 0;
  }
  ```

* 如需为事件处理方法提供其他参数，此方法也很有用（Lambda）

  ```c#
  @page "/counter"
  @inject IJSRuntime JS
  
  <h1>Counter</h1>
  
  <p id="currentCount">Current count: @currentCount</p>
  
  <button class="btn btn-primary" @onclick='mouseEvent => HandleClick(mouseEvent, "Hello")'>Click me</button>
  
  @code {
      private int currentCount = 0;
  
      private async Task HandleClick(MouseEventArgs e, string msg)
      {
          if (e.CtrlKey) // Ctrl key pressed as well
          {
              await JS.InvokeVoidAsync("alert", msg);
              currentCount += 5;
          }
          else
          {
              currentCount++;
          }
      }
  }
  ```

### 替代事件的默认 DOM 操作

* 多个 DOM 事件具有在事件发生时运行的默认操作，而无论是否有可用于该事件的事件处理程序

  ```c#
  <input value=@data @onkeypress="ProcessKeyPress"/>
  
  @code {
      private string data;
  
      private async Task ProcessKeyPress(KeyboardEventArgs e)
      {
          if (e.Key == "@")
          {
              await JS.InvokeVoidAsync("alert", "You pressed @");
          }
          else
          {
              data += e.Key.ToUpper();
          }
      }
  }
  ```

  * 如果运行此代码并按了 `@` 键，将显示警报，但 `@` 字符也将添加到输入中。 添加 `@` 字符是事件的默认操作

* 如果要禁止该字符出现在输入框中，可以使用事件的 `preventDefault` 属性替代默认操作：`<input value=@data @onkeypress="ProcessKeyPress" @onkeypress:preventDefault />`

  * 该事件仍会触发，但仅执行由事件处理程序定义的操作

* DOM 中子元素中的某些事件可以触发其父元素中的事件

  * `@onclick` 事件沿 DOM 树向上传播

* 可以使用事件的 `stopPropagation` 属性来减少此类向上激增的事件： `<button class="btn btn-primary" @onclick="IncrementCount" @onclick:stopPropagation>Click me</button>`

### 使用 EventCallback 处理跨组件的事件

* 子组件中的事件可使用 `EventCallback` 触发父组件中的事件处理程序方法。 回调将引用父组件中的方法。 子组件可以通过调用回调来运行该方法。 此机制类似于使用 `delegate` 来引用 C# 应用程序中的方法

* 回调可采用单个参数。 `EventCallback` 是泛型类型。 类型形参指定传递给回调的实参类型

  ```c#
  @* TextDisplay component *@
  @using WebApplication.Data;
  
  <p>Enter text:</p>
  <input @onkeypress="HandleKeyPress" value="@data" />
  
  @code {
      [Parameter]
      public EventCallback<KeyTransformation> OnKeyPressCallback { get; set; }
  
      private string data;
  
      private async Task HandleKeyPress(KeyboardEventArgs e)
      {
          KeyTransformation t = new KeyTransformation() { Key = e.Key };
          await OnKeyPressCallback.InvokeAsync(t);
          data += t.TransformedKey;
      }
  }
  
  @page "/texttransformer"
  @using WebApplication.Data;
  
  <h1>Text Transformer - Parent</h1>
  
  <TextDisplay OnKeypressCallback="@TransformText" />
  
  @code {
      private void TransformText(KeyTransformation k)
      {
          k.TransformedKey = k.Key.ToUpper();
      }
  }
  ```

  <img src="https://learn.microsoft.com/zh-cn/training/aspnetcore/blazor-improve-how-forms-work/media/2-eventcallback-flow.png" style="zoom: 50%;" />

* 如果使用适当的 `EventArgs` 参数键入回调，则可以将回调直接连接到事件处理程序，而无需使用中间方法

  ```c#
  <button @onclick="OnClickCallback">
      Click me!
  </button>
  
  @code {
      [Parameter]
      public EventCallback<MouseEventArgs> OnClickCallback { get; set; }
  }
  ```

## 利用 Blazor 表单的强大功能

### 什么是 EditForm

* `EditForm` 是一个 Blazor 组件，它在 Blazor 页面上履行 HTML 表单这一角色

### 创建具有数据绑定的 EditForm

* `<EditForm>` 元素支持使用 `Model` 参数进行数据绑定。 指定一个对象作为此形参的实参。 `EditForm` 中的输入元素可使用 `@bind-Value` 参数绑定到由模型公开的属性和字段
* `EditForm` 组件实现双向数据绑定。 表单会显示从模型中检索到的值，但用户可以在表单中更新这些值，并将其推送回该模型

### 了解 Blazor 输入控件

* Blazor 拥有自己的一组组件，旨在专用于 `<EditForm>` 元素并支持其他功能中的数据绑定。 下表列出了这些组件。 当 Blazor 呈现包含这些组件的页面时，它们将转换为表中列出的相应 HTML `<input>` 元素。 一些 Blazor 组件是通用的；类型参数由 Blazor 运行时根据绑定到元素的数据类型确定：

  | 输入组件                  | 呈现为 (HTML)             |
  | :------------------------ | :------------------------ |
  | `InputCheckbox`           | `<input type="checkbox">` |
  | `InputDate<TValue>`       | `<input type="date">`     |
  | `InputFile`               | `<input type="file">`     |
  | `InputNumber<TValue>`     | `<input type="number">`   |
  | `InputRadio<TValue>`      | `<input type="radio">`    |
  | `InputRadioGroup<TValue>` | 一组子单选按钮            |
  | `InputSelect<TValue>`     | `<select>`                |
  | `InputText`               | `<input>`                 |
  | `InputTextArea`           | `<textarea>`              |

* 这些元素中的每一个都具有由 Blazor 识别的属性，例如 `DisplayName`（用于将输入元素与标签关联）和 `@ref`（可用于保存对 C# 变量中字段的引用）。 任何无法识别的非 Blazor 属性都将按原样传递给 HTML 呈现器。 这意味着可以利用 HTML 输入元素属性。 例如，可以将 `min`、`max` 和 `step` 属性添加到 `InputNumber` 组件，它们将作为所呈现的 `<input type="number">` 元素一部分正常运行
*  一个 `EditForm` 可以包含多个 `RadioButtonGroup<TValue>` 组件，并且每个组都可以绑定到模型中的 `EditForm` 字段

### 处理窗体提交

* Blazor 支持两种类型的验证：声明性和编程性

  * 声明性验证规则在客户端上和浏览器中运行。 对于在数据传输到服务器之前执行基本的客户端验证，它们会非常有用。 
  * 对于处理声明性验证无法实现的复杂场景，服务器端验证非常有用，例如针对来自其他源的数据交叉检查字段中的数据。 

* 实际的应用程序应结合使用客户端验证和服务器端验证；客户端验证可捕获基本的用户输入错误，并防止将无效数据发送到服务器以进行处理等许多情况。 服务器端验证可确保用于保存数据的用户请求不会试图绕过数据验证并存储不完整或损坏的数据

* `EditForm` 具有三个在提交后运行的事件

  * `OnValidSubmit`：如果输入域成功通过其验证属性定义的验证规则，则会触发此事件
  * `OnInvalidSubmit`：如果表单上的任何输入域都未能通过其验证属性定义的验证，则会触发此事件
  * `OnSubmit`：无论所有输入域是否有效，提交 EditForm 时都会发生此事件

* `EditForm` 可以处理 `OnValidSubmit` 和 `OnInvalidSubmit` 事件对，也可以处理 `OnSubmit` 事件，但不能同时处理全部三个事件

* 通过向 `EditForm` 添加一个 `Submit` 按钮来触发提交。 当用户选择此按钮时，将触发由 `EditForm` 指定的提交事件

* 生成和部署过程不会检查提交事件组合是否无效，但非法选择将在运行时产生错误。 例如，如果尝试将 `OnValidSubmit` 与 `OnSubmit` 结合使用，应用程序将生成以下运行时异常：

  ```text
  Error: System.InvalidOperationException: When supplying an OnSubmit parameter to EditForm, do not also supply OnValidSubmit or OnInvalidSubmit.
  ```

* `EditForm` 使用 `EditContext` 对象跟踪作为模型的当前对象的状态，包括哪些字段已更改及其当前值

* 提交事件作为参数传递此 `EditContext` 对象

* 事件处理程序可以使用此对象中的 `Model` 字段来检索用户的输入

## 隐式验证用户输入，而无需编写验证代码

### 验证 Blazor 表单中的用户输入

* 在 Blazor 中使用 `EditForm` 组件时，有多种验证选项可供选择，而无需编写复杂的代码
  * 在模型中，可以对每个属性使用数据注释来告诉 Blazor 何时需要值以及它们应该采用什么格式
  * 在 `EditForm` 组件中，添加 **`DataAnnotationsValidator`** 组件，它将根据用户输入的值检查模型注释
  * 如果希望在提交的表单中显示所有验证消息的摘要，请使用 `ValidationSummary` 组件
  * 如果要显示特定模型属性的验证消息，请使用 `ValidationMessage` 组件

### 准备用于验证的模型

* 首先告诉 `DataAnnotationsValidator` 组件有效数据的外观。 可以通过在数据模型中使用注释属性来声明验证限制

  ```c#
  using  System.ComponentModel.DataAnnotations;
  
  public class Pizza
  {
      public int Id { get; set; }
      
      [Required]
      public string Name { get; set; }
      
      public string Description { get; set; }
      
      [EmailAddress]
      public string ChefEmail { get; set;}
      
      [Required]
      [Range(10.00, 25.00)]
      public decimal Price { get; set; }
  }
  ```

* 可以在模型中使用的其他注释包括

  * `[ValidationNever]`：如果要确保该字段从不包含在验证中，请使用此注释。
  * `[CreditCard]`：如果要记录用户的有效信用卡号，请使用此注释。
  * `[Compare]`：如果要确保模型中的两个属性匹配，请使用此注释。
  * `[Phone]`：如果要记录用户的有效电话号码，请使用此注释。
  * `[RegularExpression]`：如果通过将值与正则表达式进行比较来检查值的格式，请使用此注释。
  * `[StringLength]`：如果要检查字符串值的长度是否不超过最大长度，请使用此注释。
  * `[Url]`：如果要记录用户的有效 URL，请使用此注释。

### 向表单添加验证组件

* 要将表单配置为使用数据注释验证，请首先确保已将输入控件绑定到模型属性。 然后，在 `EditForm` 组件内的某个位置添加 `DataAnnotationsValidator` 组件。 若要显示验证生成的消息，请使用 `ValidationSummary` 组件，该组件显示表单中所有控件的所有验证消息。 如果想要在每个控件旁边显示验证消息，请使用多个 `ValidationMessage` 组件。 请记住，使用 `For` 属性将每个 `ValidationMessage` 控件与模型的特定属性相关联

  ```c#
  @page "/admin/createpizza"
  
  <h1>Add a new pizza</h1>
  
  <EditForm Model="@pizza">
      <DataAnnotationsValidator />
      <ValidationSummary />
      
      <InputText id="name" @bind-Value="pizza.Name" />
      <ValidationMessage For="@(() => pizza.Name)" />
      
      <InputText id="description" @bind-Value="pizza.Description" />
      
      <InputText id="chefemail" @bind-Value="pizza.ChefEmail" />
      <ValidationMessage For="@(() => pizza.ChefEmail)" />
      
      <InputNumber id="price" @bind-Value="pizza.Price" />
      <ValidationMessage For="@(() => pizza.Price)" />
  </EditForm>
  
  @code {
      private Pizza pizza = new();
  }
  ```

### 控制应用的表单验证

* Blazor 在两个不同的时间执行验证

  * 当用户按 Tab 键离开某个字段时，将执行字段验证。 字段验证可确保用户及早了解验证问题
  * 当用户提交表单时，将执行模型验证。 模型验证可确保不会存储无效数据

* 如果表单验证失败，消息将显示在 `ValidationSummary` 和 `ValidationMessage` 组件中。 若要自定义这些消息，可以为模型中每个字段的数据注释添加一个 `ErrorMessage` 属性

  ```c#
  [Required(ErrorMessage = "You must set a name for your pizza.")]
  ```

* 可以创建自定义验证Attribute。 首先创建一个从 `ValidationAttribute` 类继承的类并替代 `IsValid` 方法

  ```c#
  public class PizzaBase : ValidationAttribute
  {
      public string GetErrorMessage() => $"Sorry, that's not a valid pizza base.";
  
      protected override ValidationResult IsValid(
          object value, ValidationContext validationContext)
      {
          if (value != "Tomato" || value != "Pesto")
          {
              return new ValidationResult(GetErrorMessage());
          }
  
          return ValidationResult.Success;
      }
  }
  
  
  public class Pizza
  {
      ... 
          
      [PizzaBase]
      public string Base { get; set; }
  }
  ```

### 在表单提交时，在服务器端处理表单验证

* 如果使用 `OnSubmit`，则不会触发其他两个事件。 可改用 `EditContext` 参数来检查是否处理输入数据

  ```c#
  @page "/admin/createpizza"
  
  <h1>Add a new pizza</a>
  
  <EditForm Model="@pizza" OnSubmit=@HandleSubmission>
      <DataAnnotationsValidator />
      <ValidationSummary />
      
      <InputText id="name" @bind-Value="pizza.Name" />
      <ValidationMessage For="@(() => pizza.Name)" />
      
      <InputText id="description" @bind-Value="pizza.Description" />
      
      <InputText id="chefemail" @bind-Value="pizza.ChefEmail" />
      <ValidationMessage For="@(() => pizza.ChefEMail)" />
      
      <InputNumber id="price" @bind-Value="pizza.Price" />
      <ValidationMessage For="@(() => pizza.Price" />
  </EditForm>
  
  @code {
      private Pizza pizza = new();
      
      void HandleSubmission(EditContext context)
      {
          bool dataIsValid = context.Validate();
          if (dataIsValid)
          {
              // Store valid data here
          }
      }
  }
  ```

* 如果改用 `OnValidSubmit` 和 `OnInvalidSubmit`，则不必在每个事件处理程序中检查验证状态

  ```c#
  @page "/admin/createpizza"
  
  <h1>Add a new pizza</a>
  
  <EditForm Model="@pizza" OnValidSubmit=@ProcessInputData OnInvalidSubmit=@ShowFeedback>
      <DataAnnotationsValidator />
      <ValidationSummary />
      
      <InputText id="name" @bind-Value="pizza.Name" />
      <ValidationMessage For="@(() => pizza.Name)" />
      
      <InputText id="description" @bind-Value="pizza.Description" />
      
      <InputText id="chefemail" @bind-Value="pizza.ChefEmail" />
      <ValidationMessage For="@(() => pizza.ChefEMail)" />
      
      <InputNumber id="price" @bind-Value="pizza.Price" />
      <ValidationMessage For="@(() => pizza.Price" />
  </EditForm>
  
  @code {
      private Pizza pizza = new();
      
      void ProcessInputData(EditContext context)
      {
          // Store valid data here
      }
      
      void ShowFeedback(EditContext context)
      {
          // Take action here to help the user correct the issues
      }
  }
  ```

## JavaScript 与 Blazor 的互操作性

### 在 Blazor 应用中加载 JavaScript 代码

* 可以在 `Pages/_Host.cshtml` 文件或 `wwwroot/index.html` 文件中的现有 `<script src="_framework/blazor.*.js"></script>` 标记后添加 `<script>` 标记，具体取决于 Blazor 托管模型
* 最好不要将脚本放在页面的 `<head>` 元素中。 Blazor 仅控制 HTML 页面的 `<body>` 元素中的内容，因此如果脚本依赖于 Blazor，则 JS 互操作可能会失败。 此外，页面显示可能更慢，因为分析 JavaScript 代码所花的时间
* 将 JavaScript 文件放置在 Blazor 项目的 wwwroot 文件夹下
* 另一种选择是将引用 JavaScript 文件的 `<script>` 元素动态注入 `Pages/_Host.cshtml` 页面

### 从 .NET 代码调用 JavaScript

* 使用 [IJSRuntime](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.jsinterop.ijsruntime) 从 .NET 代码调用 JavaScript 函数

* 若要使 JS 互操作运行时可用，请将 `IJSRuntime` 抽象实例注入 Blazor 页面，在文件开始附近的 `@page` 指令之后

  ```c#
  @page "/counter"
  @inject IJSRuntime JS
  ```

* `IJSRuntime` 接口公开用于调用 JavaScript 代码的 [InvokeAsync](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.jsinterop.ijsruntime.invokeasync) 和 [InvokeVoidAsync](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.jsinterop.jsruntimeextensions.invokevoidasync) 方法

  * 使用 `InvokeAsync<TValue>` 调用返回值的 JavaScript 函数。 否则，调用 `InvokeVoidAsync`

* `InvokeAsync` 或 `InvokeVoidAsync` 方法的参数是要调用的 JavaScript 函数的名称，后跟函数所需的任何参数。 JavaScript 函数必须属于 `window` 作用域或 `window` 子作用域。 参数必须可序列化为 JSON

* JS 互操作仅在 Blazor Server 应用与浏览器建立 SignalR 连接时可用。 在呈现完成之前，无法进行互操作调用。 若要检测呈现是否已完成，请在 Blazor 代码中使用 [OnAfterRender](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.onafterrender)或 [OnAfterRenderAsync](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.onafterrenderasync) 事件。

### 使用 ElementReference 对象更新 DOM

* 许多第三方 JavaScript 库可用于在页面上呈现元素，这些库可以更新 DOM。 如果 JavaScript 代码修改了 DOM 的元素，则 DOM 的 Blazor 副本可能不再匹配当前状态。 此情况可能导致意外的行为，并可能会带来安全风险。 请勿作出可能导致 DOM 的 Blazor 视图损坏的更改。
* 处理这种情况的最简单方法是在 Blazor 组件中创建一个占位符元素，通常是空的 `<div @ref="placeHolder"></div>` 元素。 Blazor 代码会将此代码解释为空白，而 Blazor 呈现树不会尝试跟踪其内容。 可以随意向此 `<div>` 添加 JavaScript 代码元素，Blazor 不会尝试更改它。
* Blazor 应用代码定义 [ElementReference](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.elementreference) 类型的字段，用于保存对 `<div>` 元素的引用。 `<div>` 元素上的 `@ref` 属性设置字段的值。 然后，`ElementReference` 对象将传递到 JavaScript 函数，该函数可以使用引用将内容添加到 `<div>` 元素

### 从 JavaScript 调用 .NET 代码

* JavaScript 代码可以使用 `DotNet` 实用工具类（JS 互操作库的一部分）运行 Blazor 代码定义的 .NET 方法。 `DotNet` 类公开了 `invokeMethod` 和 `invokeMethodAsync` 帮助程序函数。 使用 `invokeMethod` 运行方法并等待结果，或使用 `invokeMethodAsync` 异步调用方法。 `invokeMethodAsync` 方法返回 JavaScript `Promise`
* 若要保持应用程序的响应能力，请将 .NET 方法定义为 `async`，并使用 JavaScript 中的 `invokeMethodAsync` 调用它。
* 必须使用 [JSInvokableAttribute](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.jsinterop.jsinvokableattribute) 标记要调用的 .NET 方法。 该方法必须是 `public`，并且任何参数都必须可序列化为 JSON。 此外，对于异步方法，返回类型必须是 `void`、`Task` 或泛型 `Task<T>` 对象，其中 `T` 是 JSON 可序列化类型
* 若要调用 `static` 方法，请提供包含该类的 .NET 程序集的名称、该方法的标识符以及该方法接受作为 `invokeMethod` 或 `invokeMethodAsync` 函数的参数的任何参数。 默认情况下，方法标识符与方法名称相同，但可以使用 `JSInvokable` 属性指定不同的值

### 从 JavaScript 调用 .NET 实例方法

* 若要运行实例方法，JavaScript 需要指向实例的对象引用。 JS 互操作提供了泛型 [DotNetObjectReference](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.jsinterop.dotnetobjectreference) 类型，可用于在 .NET 代码中创建对象引用。 代码必须使此对象引用可供 JavaScript 使用
* 然后，JavaScript 代码可以使用 .NET 方法的名称和该方法所需的任何参数调用 `invokeMethodAsync`。 若要避免内存泄漏，.NET 代码应在不再需要对象引用时将其释放

## 了解 Blazor 组件生命周期

* Blazor 组件具有定义完善的生命周期，该生命周期从首次创建时开始，在销毁时结束

### Blazor 组件生命周期

* 所有 Blazor 组件都源自 [ComponentBase](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase) 类或 [IComponent](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.icomponent)，该类定义了显示的方法并提供了默认行为。 通过重写相应的方法来处理事件

  <img src="https://learn.microsoft.com/zh-cn/training/aspnetcore/blazor-build-rich-interactive-components/media/4-component-lifecycle.png"  />

* 尽管该图暗示生命周期方法之间存在单线程流，但这些方法的异步版本使 Blazor 应用能够加快呈现过程。 例如，当 `SetParametersAsync` 中发生第一个 `await` 时，Blazor 组件会运行 `OnInitialized` 和 `OnInitializedAsync` 方法。 当等待的语句完成时，`SetParametersAsync` 中的执行线程将继续
* 相同的逻辑适用于一系列生命周期方法。 此外，在 `OnInitializedAsync` 和 `OnParametersSetAsync` 期间发生的每个 `await` 操作都指示组件的状态已更改，并且可以触发页面的立即呈现。 在初始化完全完成之前，页面可能会呈现多次

### 了解生命周期方法

* 每个组件生命周期方法都有特定的用途，你可以重写这些方法以向组件添加自定义逻辑。 下表列出了生命周期方法的发生顺序，并描述了其用途。

  | 订单  | 生命周期方法                                                 | 说明                                                         |
  | :---- | :----------------------------------------------------------- | :----------------------------------------------------------- |
  | **1** | 已创建组件                                                   | 组件已实例化。                                               |
  | **2** | [SetParametersAsync](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.setparametersasync) | 设置呈现树中组件的父级中的参数。                             |
  | **3** | [OnInitialized](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.oninitialized) / [OnInitializedAsync](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.oninitializedasync) | 在组件已准备好启动时发生。                                   |
  | **4** | [OnParametersSet](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.onparametersset) / [OnParametersSetAsync](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.onparameterssetasync) | 在组件收到参数且已分配属性时发生。                           |
  | **5** | [OnAfterRender](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.onafterrender) / [OnAfterRenderAsync](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.onafterrenderasync) | 在呈现组件后发生。                                           |
  | **6** | `Dispose` / `DisposeAsync`                                   | 如果组件实现 [IDisposable](https://learn.microsoft.com/zh-cn/dotnet/api/system.idisposable) 或 [IAsyncDisposable](https://learn.microsoft.com/zh-cn/dotnet/api/system.iasyncdisposable)，则会在销毁组件的过程中发生适当的可释放操作。 |

### SetParametersAsync 方法

* 当用户访问包含 Blazor 组件的页面时，Blazor 运行时会创建该组件的新实例并运行默认构造函数。 构建组件后，Blazor 运行时会调用 `SetParametersAsync` 方法
* 如果组件定义了任何参数，Blazor 运行时会将这些参数的值从调用环境注入到组件中。 这些参数包含在 `ParameterView` 对象中，可供 `SetParametersAsync` 方法访问。 你调用 `base.SetParametersAsync` 方法以使用这些值填充组件的 `Parameter` 属性
* 或者，如果你需要以不同的方式处理参数，可以在此方法处执行相应的操作。 例如，你可能需要在使用之前验证传递给组件的任何参数
* 当创建组件时，即使该组件没有任何参数，`SetParametersAsync` 方法也始终会运行

### OnInitialized 和 OnInitializedAsync 方法

* 你可以重写 `OnInitialized` 和 `OnInitializedAsync` 方法以包含自定义功能。 这些方法在 `SetParametersAsync` 方法填充组件基于参数的属性（特性为 [ParameterAttribute](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.parameterattribute) 或 [CascadingParameterAttribute](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.cascadingparameterattribute) 的属性）后运行。 可以在这些方法中运行初始化逻辑
* 如果应用程序的 `render-mode` 属性设置为 `Server`，那么 `OnInitialized` 和 `OnInitializedAsync` 方法只对组件实例运行一次。 如果组件的父级修改了组件参数，`SetParametersAsync` 方法会再次运行，但这些方法不会。 如果需要在参数更改时重新初始化组件，请使用 `SetParametersAsync` 方法。 如果要执行一次初始化，请使用这些方法
* 如果 `render-mode` 属性设置为 [ServerPrerendered](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.mvc.rendering.rendermode#microsoft-aspnetcore-mvc-rendering-rendermode-serverprerendered)，则 `OnInitialized` 和 `OnInitializedAsync` 方法将运行两次；一次在生成静态页面输出的预呈现阶段运行，另一次在服务器与浏览器建立 SignalR 连接时运行。 你可能会在这些方法中执行成本高昂的初始化任务（例如从 Web 服务检索数据，该服务用于设置 Blazor 组件的状态）。 在此类情况下，请在第一次执行期间缓存状态信息，并在第二次执行期间重用保存的状态
* 创建实例后但在 `OnInitialized` 或 `OnInitializedAsync` 方法运行之前，将注入 Blazor 组件使用的依赖项。 可以在 `OnInitialized` 或 `OnInitializedAsync` 方法中使用这些依赖项注入的对象，但不能在之前使用
* Blazor 组件不支持构造函数依赖项注入。 请改用组件标记中的 `@inject` 指令或属性声明中的 [InjectAttribute](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.injectattribute)
* 在预呈现阶段，Blazor Server 组件中的代码无法执行需要连接到浏览器的操作，例如调用 JavaScript 代码。 应将依赖于与浏览器连接的逻辑放置在 `OnAfterRender` 或 `OnAfterRenderAsync` 方法中

### OnParametersSet 和 OnParametersSetAsync 方法

* 如果这是第一次呈现组件，则 `OnParametersSet` 和 `OnParametersSetAsync` 方法在 `OnInitialized` 或 `OnInitializedAsync` 方法之后运行，否则在 `SetParametersAsync` 方法之后运行。 与 `SetParametersAsync` 一样，即使组件没有参数，也会始终调用这些方法
* 使用任一方法完成依赖于组件参数值的初始化任务，例如计算计算属性的值。 请勿在构造函数中执行此类长时间运行的操作。 构造函数是同步的，等待长时间运行的操作完成会影响包含该组件的页面的响应能力

### OnAfterRender 和 OnAfterRenderAsync 方法

* 每次 Blazor 运行时需要更新由用户界面中的组件表示的视图时，`OnAfterRender` 和 `OnAfterRenderAsync` 方法都会运行
* 在以下情况下会自动出现此状态
  * 组件的状态更改，例如当 `OnInitialized` 或 `OnInitializedAsync` 方法或者 `OnParametersSet` 和 `OnParametersSetAsync` 方法运行时。
  * 触发 UI 事件。
  * 应用程序代码调用组件的 `StateHasChanged` 方法。
* 从外部事件或 UI 触发器调用 [StateHasChanged](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.statehaschanged) 时，组件会有条件地重新呈现。 以下列表详细介绍了方法调用的顺序，包括 `StateHasChanged` 和以下内容
  1. [StateHasChanged](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.statehaschanged)：组件被标记为需要重新呈现。
  2. [ShouldRender](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.shouldrender)：返回一个标志，指示组件是否应呈现。
  3. [BuildRenderTree](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.componentbase.buildrendertree)：呈现组件。
* `StateHasChanged ` 方法调用组件的 `ShouldRender` 方法。 此方法的目的是判断状态的更改是否需要组件重新呈现视图。 默认情况下，所有状态更改都会触发呈现操作，但你可以重写 `ShouldRender` 方法并定义自己的决策逻辑。 如果视图需要再次呈现，`ShouldRender` 方法将返回 `true`，否则返回 `false`
* 如果组件需要呈现，可以使用 `BuildRenderTree` 方法生成一个模型，该模型可用于更新浏览器用来显示 UI 的 DOM 版本。 你可以使用 `ComponentBase` 类提供的默认方法实现，或者如果你有特定要求，也可以使用自定义逻辑进行重写
* 接下来，呈现组件视图并更新 UI。 最后，该组件运行 `OnAfterRender` 和 `OnAfterRenderAsync` 方法。 此时，UI 功能齐全，你可以与 JavaScript 和 DOM 中的任何元素进行交互。 使用这些方法执行需要访问完全呈现的内容的任何其他步骤，例如从 JS 互操作调用 JavaScript 代码
* `OnAfterRender` 和 `OnAfterRenderAsync` 方法采用名为 `firstRender` 的布尔参数。 该参数在方法首次运行时为 `true`，但之后为 `false`。 可以计算此参数来执行一次性操作，如果每次呈现组件时都重复这些操作，则可能会造成浪费和过多资源消耗
* 不要将预呈现与 Blazor 组件的首次呈现混淆。 预呈现发生在与浏览器建立 SignalR 连接之前，且生成页面的静态版本。 首次呈现发生于与浏览器的连接完全处于活动状态并且所有功能都可用时

### Dispose 和 DisposeAsync 方法

* 与任何 .NET 类一样，Blazor 组件可以使用托管和非托管资源。 运行时会自动回收受管理资源。 但是，应实现 `IDisposable` 或 `IAsyncDisposable` 接口，并提供 `Dispose` 或 `DisposeAsync` 方法来释放任何不受管理的资源。 这样的做法可以减少服务器中内存泄漏的可能性

### 处理生命周期方法中的异常

* 如果 Blazor 组件的生命周期方法失败，它会关闭与浏览器的 SignalR 连接，从而导致 Blazor 应用停止运行。 若要防止出现此结果，请确保已准备好将异常作为生命周期方法逻辑的一部分进行处理。 有关详细信息，请参阅[处理 ASP.NET Core Blazor 应用中的错误](https://learn.microsoft.com/zh-cn/aspnet/core/blazor/fundamentals/handle-errors)。

## 了解模板组件

* 模板组件可跨多个应用重复使用，为 UI 元素自定义提供经过尝试和测试的布局和逻辑的基础
* 模板组件定义常见元素并将其应用于所有页面，从而跨 Web 应用应用标准化设计
* 模板可以简化更新（例如品牌重塑），因为只需在中心模板位置进行修改

### RenderFragment 类型

* 模板组件为一个或多个 HTML 标记片段提供布局和逻辑。 HTML 使用模板组件提供的上下文呈现。 模板组件使用 [RenderFragment](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.renderfragment) 对象作为占位符，在运行时将标记插入其中
* 模板只是一个普通的 Razor 组件。 若要使用模板，使用组件会像引用任何其他组件一样引用模板。 名称 `ChildContent` 是 `RenderFragment` 参数的默认名称。 可以为参数指定不同的名称，但在测试页应用模板时必须指定此名称

### 泛型 RenderFragment<T> 参数

* 默认情况下，`RenderFragment` 类充当 HTML 标记块的占位符。 但是，你可以使用泛型类型 [RenderFragment](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.components.renderfragment-1) 通过类型参数呈现其他类型的内容，并提供逻辑来处理模板组件中的指定类型
* 例如，假设你要创建一个模板来显示集合中的项。 可以使用 C# `foreach` 循环来循环访问集合并显示找到的项目。 但是该集合可能包含任何类型的数据，因此需要一种呈现每个项的通用方法
* 若要编写泛型类型模板组件，需要在模板组件本身和模板的使用组件中指定类型参数。 以下列表表示泛型类型模板组件的常见特征
  * 模板组件中的类型参数是使用 `@typeparam` 指令引入的。 如有必要，一个模板组件可以有多个类型参数。
  * 模板可能会定义一个参数，该参数包含由类型参数所指定类型的可枚举对象集合。
  * 模板还根据采用相同类型参数的泛型 `RenderFragment` 类型定义 `ChildContent` 参数。

## Razor 类库创建和概念

* Web 应用程序中的组件使开发人员能够在整个应用程序中重用部分应用程序用户界面。 通过 Razor 类库，开发人员可在多个应用程序之间共享和重用这些组件

### 关于 Razor 类库

* Razor 类库是一种 .NET 项目类型，它包含 Razor 组件、页面、HTML、级联样式表 (CSS) 文件、JavaScript、图像和其他可由 Blazor 应用程序引用的静态 Web 内容
* 与其他 .NET 类库项目一样，Razor 类库可以捆绑为 NuGet 包并在 NuGet 包存储库（如 NuGet.org）上共享

### 使用默认模板创建项目

* 可以在 Visual Studio 中通过选择“文件”>“新建项目”来开始创建 Razor 类库
* 还可以在命令行接口上通过运行以下命令来创建项目：`dotnet new razorclasslib -o MyProjectName`

### 静态资产交付

* wwwroot 文件夹的内容可在该文件夹的其他内容和组件单独的 CSS 文件（如 Component1.razor.css）中作为同一基文件夹中的文件进行相对引用。 例如，默认 CSS 会添加双像素虚线红色边框和背景图像样式，后者将使用 wwwroot 文件夹中 background.png 图像。 无需路径即可从 CSS 引用驻留在 wwwroot 文件夹中的内容。

  ```css
  /* background.png 在 wwwroot 文件夹下 */
  
  .my-component {
      border: 2px dashed red;
      padding: 1em;
      margin: 1em 0;
      background-image: url('background.png');
  }
  ```

* wwwroot 文件夹的内容可供托管 Blazor 应用程序引用，绝对文件夹引用的格式如下：

  ```css
  /_content/{PACKAGE_ID}/{PATH_AND_FILENAME_INSIDE_WWWROOT}
  ```





