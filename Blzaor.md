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

在 Blazor 中，组件之间可以通过以下三种方式共享信息：

1. **组件参数（[Parameter]）**：用于 **父组件向子组件传递数据**。
2. **级联参数（[CascadingParameter]）**：适用于 **多层级组件共享数据**，避免手动逐层传递。
3. **AppState 共享状态**：用于 **全局状态管理**，使数据在整个应用中共享。

### 1. 使用组件参数（[Parameter]）共享信息

适用于 **父组件向子组件传递数据**。

#### **基本用法**

- 在 **子组件** 中，定义 **带** **`[Parameter]`** **特性的公共属性**，用于接收数据。
- 在 **父组件** 中，直接通过 **属性赋值** 传递数据。

#### **示例：字符串参数**

```c#
@code {
    [Parameter]
    public string PizzaName { get; set; }

	// 如果父组件不传递值，将呈现此值。 否则，它将被从父组件传递的值替代
    [Parameter]
    public string PizzaDescription { get; set; } = "The best pizza you've ever tasted.";
}
```

#### **示例：对象参数**

```c#
@code {
    [Parameter]
    public PizzaTopping Topping { get; set; }
}
```

#### **示例：父组件传递参数**

```c#
<Pizza PizzaName="Hawaiian" PizzaDescription="The one with pineapple" />
<PizzaTopping Topping="@(new PizzaTopping() { Name = "Chilli Sauce", Ingredients = "Three kinds of chilli." })" />
```

### 2. 使用级联参数（[CascadingParameter]）共享信息

适用于 **多层级组件共享数据**，避免手动逐层传递。

#### **基本用法**

- 在 **父组件** 中，使用 `<CascadingValue>` 提供级联数据。
- 在 **子组件** 中，使用 `[CascadingParameter]` 接收数据。

#### **示例：父组件提供级联参数**

```c#
<CascadingValue Name="DealName" Value="Throwback Thursday">
    <!-- 这里的所有子组件都能访问 "DealName" 这个值 -->
</CascadingValue>
```

#### **示例：子组件接收级联参数**

```c#
@code {
    [CascadingParameter(Name="DealName")]
    private string DealName { get; set; }
}
```

**注意**：如果只有一个该类型的参数，可省略 `Name` 进行 **类型匹配**。

### 3. 使用 AppState 共享信息（全局状态管理）

适用于 **多个非层级组件之间共享数据**。

#### **基本用法**

1. **创建状态类**

```c#
public class PizzaSalesState
{
    public int PizzasSoldToday { get; set; }
}
```

1. **注册服务（在** `Program.cs` **中）**

```c#
builder.Services.AddScoped<PizzaSalesState>();
```

1. **在组件中注入并使用**

```c#
@inject PizzaSalesState SalesState

<p>Today, we've sold this many pizzas: @SalesState.PizzasSoldToday</p>

<button @onclick="IncrementSales">Buy a Pizza</button>

@code {
    private void IncrementSales()
    {
        SalesState.PizzasSoldToday++;
    }
}
```

### 4. 选择合适的共享方式

| 方式                                 | 适用场景                   | 组件层级          | 适用性                         |
| ------------------------------------ | -------------------------- | ----------------- | ------------------------------ |
| **组件参数（[Parameter]）**          | **父组件传递数据到子组件** | 父 → 子           | ✅ 适用于简单数据传递           |
| **级联参数（[CascadingParameter]）** | **多层级组件共享数据**     | 父 → 孙（或更深） | ✅ 避免手动逐层传递             |
| **AppState**                         | **全局状态管理**           | 任意组件          | ✅ 适用于多个非层级组件共享数据 |

### 5. 结论

- **仅在父子组件之间传递数据** → 组件参数（[Parameter]）
- **数据需在多个层级的组件中使用** → 级联参数（[CascadingParameter]）
- **数据需在整个应用中共享** → `AppState`（作用域服务）

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

### 1. 路由基础

`Blazor` 使用 `Router` 组件管理路由，通常在 `App.razor` 文件中进行配置：

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

#### 关键属性

- **`AppAssembly`**：应用启动时，`Blazor` 扫描 `AppAssembly` 以查找带有 `RouteAttribute` 的组件。

- **`<Found>`**：匹配路由时，渲染 `RouteView` 组件，该组件解析 `RouteData` 并渲染目标组件。

- **`<NotFound>`**：当找不到匹配路由时，显示自定义内容。

- **`@page` 指令**：用于指定组件的路由路径，例如 `@page "/pizzas"`。支持多个路由：

  ```c#
  @page "/Pizzas"
  @page "/CustomPizzas"
  ```

### 2. NavigationManager 导航

`NavigationManager` 提供当前 `URL` 相关信息，并可用于页面跳转。

#### 获取 `URL` 相关信息

- `NavManager.Uri`：完整 `URI`（如 `http://www.contoso.com/pizzas/margherita?extratopping=pineapple`）。
- `NavManager.BaseUri`：基本 `URI`（如 `http://www.contoso.com/`）。
- `NavManager.ToAbsoluteUri(NavManager.Uri).Query`：查询字符串（如 `?extratopping=pineapple`）。

#### 示例：获取 `BaseUri`

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

#### 解析查询字符串

```c#
@using Microsoft.AspNetCore.WebUtilities

protected override void OnInitialized()
{
    var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
    if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("extratopping", out var extraTopping))
    {
        ToppingName = System.Convert.ToString(extraTopping);
    }
}
```

#### 代码导航

使用 `NavigateTo()` 进行页面跳转：

```c#
NavManager.NavigateTo("buypizza");
```

### 3. NavLink 组件（导航链接）

`Blazor` 使用 `NavLink` 组件创建导航链接。它会在 `href` 匹配当前 `URL` 时自动添加 `active` `CSS` 类。

```c#
<NavLink href="pizzas" Match="NavLinkMatch.Prefix">Pizzas</NavLink>
```

#### `Match` 属性

- `NavLinkMatch.All`：完全匹配 `URL` 时高亮。
- `NavLinkMatch.Prefix`：`URL` 以 `href` 开头时高亮。

### 4. 路由参数

#### 基本路由参数

`http://www.contoso.com/favoritepizza/hawaiian` 传递 `hawaiian` 参数：

```c#
@page "/FavoritePizzas/{favorite}"
<p>Your favorite pizza is: @Favorite</p>

@code {
    [Parameter]
    public string Favorite { get; set; }
}
```

#### 可选路由参数

```c#
@page "/FavoritePizzas/{favorite?}"

@code {
    [Parameter]
    public string Favorite { get; set; }
    
    protected override void OnInitialized()
    {
        Favorite ??= "Fiorentina";
    }
}
```

#### 路由参数约束

```c#
@page "/FavoritePizza/{preferredsize:int}"
<p>Your favorite pizza size is: @FavoriteSize inches.</p>

@code {
    [Parameter]
    public int FavoriteSize { get; set; }
}
```

#### 常见参数约束

| 约束       | 示例                   | 匹配示例                                                   |
| ---------- | ---------------------- | ---------------------------------------------------------- |
| `bool`     | `{vegan:bool}`         | `http://www.contoso.com/pizzas/true`                       |
| `datetime` | `{birthdate:datetime}` | `http://www.contoso.com/customers/1995-12-12`              |
| `decimal`  | `{maxprice:decimal}`   | `http://www.contoso.com/pizzas/15.00`                      |
| `double`   | `{weight:double}`      | `http://www.contoso.com/pizzas/1.234`                      |
| `float`    | `{weight:float}`       | `http://www.contoso.com/pizzas/1.564`                      |
| `guid`     | `{pizzaId:guid}`       | `http://www.contoso.com/pizzas/CD2C1638-1638-DEADBEEF1638` |
| `long`     | `{totalsales:long}`    | `http://www.contoso.com/pizzas/568192454`                  |

### 5. 捕获全部路由参数

捕获多个 `URI` 片段：

```c#
@page "/FavoritePizza/{*favorites}"
<p>Your favorite pizzas are: @Favorites</p>

@code {
    [Parameter]
    public string Favorites { get; set; }
}
```

**示例**：

- `http://www.contoso.com/favoritepizza/margherita/hawaiian`
- 页面显示 `Your favorite pizzas are: margherita/hawaiian`

## Blzaor布局

### 1. **Blazor 布局概述**

- **布局组件的作用**：简化和重用通用 UI 元素。
- **布局组件与其他组件的相似性**：Blazor 布局是特定类型的组件，其编写方式与其他 UI 组件相似。

### 2. **布局组件的基本要求**

- **继承**：必须继承 `LayoutComponentBase` 类。

- **包含 `@Body` 指令**：必须在组件中指定一个位置，放置 `@Body` 指令来呈现组件内容。

  示例代码：

  ```
  c#复制编辑@inherits LayoutComponentBase
  
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

### 3. **布局组件的其他特点**

- **不需要 `@page` 指令**：布局组件不处理请求，因此不应为它们创建路由。
- **文件存放位置**：布局组件通常存放在 `Shared` 文件夹中，但也可以选择其他位置。

### 4. **布局组件的全局应用**

- **使用 `_Imports.razor` 文件简化布局应用**：

  - 在该文件中指定布局组件，将自动应用到文件夹内所有组件，免去手动添加 `@layout` 指令。
  - `_Imports.razor` 文件中的布局指令适用于该文件夹及其所有子文件夹。

  > 注意：请勿向项目的根文件夹中的 `_Imports.razor` 文件添加 `@layout` 指令，因为这会导致布局的无限循环

- **在 `App.razor` 中设置默认布局**：

  - 使用 `App.razor` 组件配置应用中的默认布局。

  - 示例代码：

    ```
    c#复制编辑<Router AppAssembly="@typeof(Program).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(BlazingPizzasMainLayout)" />
        </Found>
        <NotFound>
            <p>Sorry, there's nothing at this address.</p>
        </NotFound>
    </Router>
    ```

### 5. **布局覆盖规则**

- 如果在组件中使用了 `@layout` 指令或在 `_Imports.razor` 中指定了布局，则该组件的布局会覆盖全局默认布局设置。

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

## 案例：四子棋游戏

### 设置 Blzaor 组件的样式

* 使用 Blazor 组件中称为 CSS 隔离的功能来创建仅应用于该组件内容的样式规则。 通过创建与组件同名的文件并添加 .css 文件扩展名，Blazor 可将此命名识别为仅应用于相应组件中 HTML 内容的样式

* Blazor 组件还可使用特殊 `HeadContent` 标记定义要添加到页面 HTML 标头的内容，此 `style` 标记和内容呈现在页面的 `head` 标记内

  ```c#
  <HeadContent>
      <style>
          ...my styles here
      </style>
  </HeadContent>
  ```

### 处理 UI 事件

* 如果要处理 UI 事件，需要使用*交互式呈现模式*呈现 Blazor 组件。 默认情况下，Blazor 组件是从服务器以静态方式呈现的。 可以使用特性 `@rendermode` 将交互式呈现模式应用于组件：`<Board @rendermode="InteractiveServer" />`
* `InteractiveServer` 呈现模式会通过与浏览器的 WebSocket 连接从服务器处理组件的 UI 事件

### 在代码中表示状态

* 首先，什么是状态？ 游戏中的状态指的是游戏中发生的情况、你拥有的分数、你的游戏位置在哪里等
* 在游戏开发中谈到状态时，一项重要的指导是将状态与 UI 分开，除了其他好处以外，这样还能让你更轻松地进行修改，且让代码更易于阅读
  * 在 Blazor 的上下文中，这意味着状态和围绕状态的逻辑应位于其自己的 C# 类中

## 使用 Blazor 混合和 .NET MAUI 生成移动和桌面应用

* 通过 Blazor，C# 开发人员可运用自身技能使用 C# 生成 Web 应用。 Blazor Hybrid 使开发人员能够从本机移动和桌面客户端应用中使用 Blazor Web UI 组件（称为 Razor 组件）。 Blazor Hybrid 应用使用了 Web 和本机客户端开发的“混合”。
* Blazor Hybrid 支持通过以下方式使用 Razor 组件：
  - .NET MAUI（多平台用户界面）
  - Windows 窗体 (WinForms)
  - Windows Presentation Foundation (WPF)
* 借助 Blazor，开发人员可使用常用语言、框架和工具生成 Web 应用的前端和后端逻辑。 使用 .NET MAUI 时，可以通过单个项目构建多平台应用（包括 iOS、Android、macOS 和 Windows），并访问适用于移动和桌面平台的平台特定源代码和资源。 将这两种技术与 Blazor Hybrid 相结合后，开发人员可以构建利用共享 UI 组件和逻辑的本机客户端和 Web 应用。 他们可以将 Blazor Hybrid 用于整个本机应用程序或本机应用程序的某些部分。

### 什么是 Blazor Hybrid？

* Blazor Hybrid 使开发人员能够将桌面和移动本机客户端框架与 .NET 和 Blazor 结合使用
* 在 Blazor Hybrid 应用中，Razor 组件在设备上是本机运行的。 这些组件通过本地互操作通道呈现到嵌入式 Web 视图控件。 组件不在浏览器中运行，并且不涉及 WebAssembly。 Razor 组件可快速加载和执行代码，这些组件可通过 .NET 平台完全访问设备的本机功能。

### 什么是 .NET MAUI？

* .NET 多平台应用 UI (.NET MAUI) 是一个跨平台框架，用于使用 C# 和 XAML 创建本机移动和桌面应用。 使用 .NET MAUI，可从单个共享代码库开发可在 Android、iOS、macOS 和 Windows 上运行的应用。 .NET MAUI 的主要目的之一是使你能够在单个代码库中实现尽可能多的应用逻辑和 UI 布局。 .NET MAUI 将 Android、iOS、macOS 和 Windows API 统一到单个 API 中，提供“编写一次就能在任何地方运行”的开发人员体验，同时还提供了对每个原生平台各个方面的深入访问

  

![](https://learn.microsoft.com/zh-cn/training/aspnetcore/build-blazor-hybrid/media/dotnet-maui.png)

### 使用 .NET MAUI 的 Blazor Hybrid 应用

* Blazor Hybrid 支持内置于 .NET MAUI 框架中。 .NET MAUI 包含 BlazorWebView 控件，该控件允许将 Razor 组件呈现到嵌入式 Web 视图中。 通过结合使用 .NET MAUI 和 Blazor，可以跨移动设备、桌面设备和 Web 重复使用一组 Web UI 组件。

### .NET MAUI 项目文件

* App.xaml：此文件定义应用在 XAML 布局中使用的应用程序资源。 默认资源位于 `Resources` 文件夹中，并为 .NET MAUI 的每个内置控件定义应用范围内的颜色和默认样式。

* App.xaml.cs：App.xaml 文件的代码隐藏。 此文件定义 App 类。 此类表示运行时的应用程序。 此类中的构造函数创建一个初始窗口并将其分配给 `MainPage` 属性；此属性确定应用程序开始运行时显示哪个页面。 此外，此类让你能够替代常见的平台中性应用程序生命周期事件处理程序。 事件包括 `OnStart`、`OnResume` 和 `OnSleep`。

* MainPage.xaml：此文件包含用户界面定义。 .NET MAUI Blazor 应用模板生成的示例应用包括 `BlazorWebView`，用于在 CSS 选择器 (`#app`) 指定的位置加载指定主机 HTML 页面 (`wwwroot/index.html`) 中的 `Components.Routes` 组件。

  ```xml
  <?xml version="1.0" encoding="utf-8" ?>
  <ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
              xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
              xmlns:local="clr-namespace:BlazorHybridApp"
              x:Class="BlazorHybridApp.MainPage"
              BackgroundColor="{DynamicResource PageBackgroundColor}">
  
      <BlazorWebView x:Name="blazorWebView" HostPage="wwwroot/index.html">
          <BlazorWebView.RootComponents>
              <RootComponent Selector="#app" ComponentType="{x:Type local:Components.Routes}" />
          </BlazorWebView.RootComponents>
      </BlazorWebView>
  
  </ContentPage>
  ```

* MainPage.xaml.cs：页面的代码隐藏。 在此文件中，你为各种事件处理程序和页面上的 .NET MAUI 控件触发的其他操作定义逻辑。 模板中的示例代码仅具有默认构造函数，因为所有用户界面和事件都位于 Blazor 组件中。

  ```c#
  namespace BlazorHybridApp;
  
  public partial class MainPage : ContentPage
  {
      public MainPage()
      {
          InitializeComponent();
      }
  }
  ```

* MauiProgram.cs：每个本机平台都有一个不同的起点，用于创建和初始化应用程序。 可以在项目中的 Platforms 文件夹下找到此代码。 此代码特定于平台，但最后调用静态 `MauiProgram` 类的 `CreateMauiApp` 方法。 使用 `CreateMauiApp` 方法通过创建应用生成器对象来配置应用程序。 至少需要指定描述应用程序的类。 可以使用应用生成器对象的 `UseMauiApp` 泛型方法执行此操作，类型参数指定应用程序类。 应用生成器还提供用于注册字体、为依赖项注入配置服务、为控件注册自定义处理程序等任务的方法。 以下代码演示了使用应用生成器注册字体、注册天气服务以及通过 `AddMauiBlazorWebView` 方法添加对 Blazor Hybrid 的支持的示例：

  ```c#
  using Microsoft.AspNetCore.Components.WebView.Maui;
  using BlazorHybridApp.Data;
  
  namespace BlazorHybridApp;
  
  public static class MauiProgram
  {
      public static MauiApp CreateMauiApp()
      {
          var builder = MauiApp.CreateBuilder();
          builder
          .UseMauiApp<App>()
          .ConfigureFonts(fonts =>
          {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          });
  
          builder.Services.AddMauiBlazorWebView();
  
          #if DEBUG
          builder.Services.AddBlazorWebViewDeveloperTools();
          builder.Logging.AddDebug();
          #endif
  
          return builder.Build();
      }
  }
  ```

### Blazor Hybrid 中的数据绑定和事件

*  在 Blazor 应用中，可在单独的 .cs 文件中添加 C# 代码，也可在 Razor 组件中添加内联
