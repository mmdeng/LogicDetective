namespace LogicDetective.Tests;

public class HintTrackerTests
{
    [Fact]
    public void GetNext_ReturnsEachHintOnlyOnce()
    {
        var tracker = new HintTracker();
        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");
        var clue = new SameClue(firstItem, secondItem);
        var relations = new[] { clue };

        Assert.Same(clue, tracker.GetNext(relations));
        Assert.Null(tracker.GetNext(relations));
    }

    [Fact]
    public void GetNext_ReturnsNoHintWhenRelationsAreEmpty()
    {
        var tracker = new HintTracker();
        var result = tracker.GetNext(Array.Empty<Clue>());
        Assert.Null(result);
    }

    [Fact]
    public void GetNext_ReturnsHintsInOrder()
    {
        var tracker = new HintTracker();
        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");
        var thirdItem = new Item(1, 1, "Y");
        var firstClue = new SameClue(firstItem, secondItem);
        var secondClue = new DifferentClue(firstItem, thirdItem);
        var relations = new Clue[] { firstClue, secondClue };

        Assert.Same(firstClue, tracker.GetNext(relations));
        Assert.Same(secondClue, tracker.GetNext(relations));
        Assert.Null(tracker.GetNext(relations));
    }

    [Fact]
    public void GetNext_ReturnsNewHintWhenRelationsAreUpdated()
    {
        var tracker = new HintTracker();
        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");
        var thirdItem = new Item(1, 1, "Y");

        var firstClue = new SameClue(firstItem, secondItem);
        var secondClue = new DifferentClue(firstItem, thirdItem);
        Assert.Same(firstClue, tracker.GetNext([firstClue]));

        var relations = new Clue[] { firstClue, secondClue };
        Assert.Same(secondClue, tracker.GetNext(relations));
    }

    [Fact]
    public void GetNext_ReturnsNullAfterAllHintsHaveBeenShown()
    {
        var tracker = new HintTracker();
        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");
        var thirdItem = new Item(1, 1, "Y");

        var firstClue = new SameClue(firstItem, secondItem);
        var secondClue = new DifferentClue(firstItem, thirdItem);
        var relations = new Clue[] { firstClue, secondClue };

        Assert.Same(firstClue, tracker.GetNext(relations));
        Assert.Same(secondClue, tracker.GetNext(relations));
        Assert.Null(tracker.GetNext(relations));
    }
}