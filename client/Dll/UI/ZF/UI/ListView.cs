using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ZF.UI
{
	[AddComponentMenu("UI/ListView")]
	[RequireComponent(typeof(ScrollRect))]
	public class ListView : MonoBehaviour
	{
		private enum Layout
		{
			Vertical,
			Horizontal
		}

		public Action<GameObject> RemoveMethod = delegate(GameObject item)
		{
			Object.Destroy((Object)(object)item);
		};

		[Tooltip("内部组件，请勿修改。")]
		[SerializeField]
		private ScrollRect _scrollRect;

		[Tooltip("内部组件，请勿修改。")]
		[SerializeField]
		private HorizontalOrVerticalLayoutGroup _content;

		[Header("列表布局")]
		[Tooltip("使用垂直List View还是水平List View？默认为垂直List View。")]
		[SerializeField]
		private Layout _layout;

		[Tooltip("List View的内边距。")]
		[SerializeField]
		private RectOffset _padding;

		[Tooltip("List View的行间距。")]
		[Range(0f, 5000f)]
		[SerializeField]
		private float _spacing;

		[Header("元素属性")]
		[Tooltip("新增的元素是否添加到List View的头部？默认将新增元素添加到List View的尾部。")]
		public bool NewElementOnTop;

		[Tooltip("List View中所有元素的尺寸是否相同？如果不是，则每次添加和移除元素时计算元素尺寸。")]
		public bool FixedElementLength = true;

		[Tooltip("添加和移除List View元素时的动画时长，小于等于0时不播放动画。")]
		[Range(0f, 1f)]
		public float AnimationTime = 0.2f;

		private float _itemLenght = -1f;

		private List<GameObject> _items = new List<GameObject>();

		public RectOffset Padding
		{
			get
			{
				return _padding;
			}
			set
			{
				_padding = value;
				((LayoutGroup)_content).set_padding(_padding);
				CalcContentLength();
			}
		}

		public float Spacing
		{
			get
			{
				return _spacing;
			}
			set
			{
				_spacing = value;
				_content.set_spacing(_spacing);
				CalcContentLength();
			}
		}

		public int ItemCount => _items.Count;

		public ListView()
			: this()
		{
		}

		public void init()
		{
			InitLayout();
			((LayoutGroup)_content).set_padding(_padding);
			_content.set_spacing(_spacing);
			CalcContentLength();
		}

		public void validate()
		{
			InitLayout();
			((LayoutGroup)_content).set_padding(_padding);
			_content.set_spacing(_spacing);
			CalcContentLength();
		}

		public void AddItem(GameObject item, int index = -1)
		{
			item.get_transform().SetParent(((Component)_content).get_transform());
			if (index < 0)
			{
				if (NewElementOnTop)
				{
					_items.Insert(0, item);
					item.get_transform().SetAsFirstSibling();
				}
				else
				{
					_items.Add(item);
					item.get_transform().SetAsLastSibling();
				}
			}
			else
			{
				if (index > ItemCount)
				{
					index = ItemCount;
					Debug.LogWarningFormat("ListView.AddItem()：给定索引超出ListView元素数量，被自动裁剪为【{0}】。", new object[1] { index });
				}
				_items.Insert(index, item);
				item.get_transform().SetSiblingIndex(index);
			}
			AdjustContentLength(item.get_transform(), 1);
			if (AnimationTime > 0f)
			{
				((MonoBehaviour)this).StartCoroutine(IEAddItemAnim(item));
			}
		}

		public void AddItemToTop(GameObject item)
		{
			AddItem(item, 0);
		}

		public void AddItemToBottom(GameObject item)
		{
			AddItem(item, ItemCount);
		}

		public bool RemoveItem(GameObject item)
		{
			bool flag = _items.Contains(item);
			if (flag)
			{
				_items.Remove(item);
				AdjustContentLength(item.get_transform(), -1);
				RemoveListItem(item);
			}
			return flag;
		}

		public bool RemoveItem(int index)
		{
			if (index < 0 || index >= _items.Count)
			{
				Debug.LogWarningFormat("ListView.RemoveListItem()：没有索引为【{0}】的元素，移除失败。", new object[1] { index });
				return false;
			}
			GameObject val = _items[index];
			_items.RemoveAt(index);
			AdjustContentLength(val.get_transform(), -1);
			RemoveListItem(val);
			return true;
		}

		public void RemoveTop(int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				RemoveItem(0);
			}
		}

		public void RemoveBottom(int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				RemoveItem(ItemCount - 1);
			}
		}

		public int RemoveAllItems()
		{
			int count = _items.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				GameObject val = _items[num];
				_items.RemoveAt(num);
				AdjustContentLength(val.get_transform(), -1);
				RemoveListItem(val);
			}
			return count;
		}

		public GameObject GetItem(int index)
		{
			return _items[index];
		}

		public GameObject FindItem(Func<GameObject, bool> check)
		{
			GameObject result = null;
			foreach (GameObject item in _items)
			{
				if (check(item))
				{
					return item;
				}
			}
			return result;
		}

		public GameObject[] FindItems(Func<GameObject, bool> check)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject item in _items)
			{
				if (check(item))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public void LocateTo(int index)
		{
			if (index <= 0)
			{
				LocateTo(0f);
			}
			else if (index >= ItemCount)
			{
				LocateTo(1f);
			}
			else
			{
				LocateTo(((float)index + 1f) / (float)ItemCount);
			}
		}

		public void LocateTo(float percent)
		{
			percent = Mathf.Clamp01(percent);
			if (_layout == Layout.Vertical)
			{
				_scrollRect.set_verticalNormalizedPosition(percent);
			}
			else
			{
				_scrollRect.set_horizontalNormalizedPosition(percent);
			}
		}

		public void Sort(Comparison<GameObject> comparison)
		{
			_items.Sort(comparison);
			for (int i = 0; i < _items.Count; i++)
			{
				_items[i].get_transform().SetSiblingIndex(i);
			}
		}

		private void InitLayout()
		{
			VerticalLayoutGroup componentInChildren = ((Component)this).GetComponentInChildren<VerticalLayoutGroup>(false);
			HorizontalLayoutGroup componentInChildren2 = ((Component)this).GetComponentInChildren<HorizontalLayoutGroup>(false);
			_scrollRect = ((Component)this).GetComponent<ScrollRect>();
			if (_layout == Layout.Vertical)
			{
				_scrollRect.set_vertical(true);
				_scrollRect.set_horizontal(false);
				_003F val = _scrollRect;
				Transform transform = ((Component)componentInChildren).get_transform();
				((ScrollRect)val).set_content((RectTransform)(object)((transform is RectTransform) ? transform : null));
				((Component)componentInChildren).get_gameObject().SetActive(true);
				_content = (HorizontalOrVerticalLayoutGroup)(object)componentInChildren;
			}
			else
			{
				_scrollRect.set_vertical(false);
				_scrollRect.set_horizontal(true);
				_003F val2 = _scrollRect;
				Transform transform2 = ((Component)componentInChildren2).get_transform();
				((ScrollRect)val2).set_content((RectTransform)(object)((transform2 is RectTransform) ? transform2 : null));
				((Component)componentInChildren2).get_gameObject().SetActive(true);
				_content = (HorizontalOrVerticalLayoutGroup)(object)componentInChildren2;
			}
		}

		private void CalcContentLength()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			int childCount = ((Component)_content).get_transform().get_childCount();
			num += (float)(childCount - 1) * _spacing;
			IEnumerator enumerator = ((Component)_content).get_transform().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					RectTransform val = (RectTransform)((current is RectTransform) ? current : null);
					num = ((_layout != 0) ? (num + val.get_sizeDelta().x) : (num + val.get_sizeDelta().y));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			num = ((_layout != 0) ? (num + (float)(_padding.get_left() + _padding.get_right())) : (num + (float)(_padding.get_top() + _padding.get_bottom())));
			Transform transform = ((Component)_content).get_transform();
			RectTransform val2 = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			Vector2 sizeDelta = val2.get_sizeDelta();
			if (_layout == Layout.Vertical)
			{
				sizeDelta.y = num;
			}
			else
			{
				sizeDelta.x = num;
			}
			val2.set_sizeDelta(sizeDelta);
		}

		private void AdjustContentLength(Transform itemTrans, int power)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			if (!FixedElementLength || _itemLenght < 0f)
			{
				RectTransform val = (RectTransform)(object)((itemTrans is RectTransform) ? itemTrans : null);
				if (_layout == Layout.Vertical)
				{
					_itemLenght = val.get_sizeDelta().y;
				}
				else
				{
					_itemLenght = val.get_sizeDelta().x;
				}
			}
			power = ((power >= 0) ? 1 : (-1));
			Transform transform = ((Component)_content).get_transform();
			RectTransform val2 = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			Vector2 sizeDelta = val2.get_sizeDelta();
			if (_layout == Layout.Vertical)
			{
				sizeDelta.y += (_itemLenght + _spacing) * (float)power;
			}
			else
			{
				sizeDelta.x += (_itemLenght + _spacing) * (float)power;
			}
			val2.set_sizeDelta(sizeDelta);
		}

		private void RemoveListItem(GameObject item)
		{
			if (AnimationTime > 0f)
			{
				((MonoBehaviour)this).StartCoroutine(IERemoveItemAnim(item));
				return;
			}
			item.get_transform().SetParent((Transform)null);
			RemoveMethod(item);
		}

		private IEnumerator IEAddItemAnim(GameObject item)
		{
			Transform transform = item.get_transform();
			RectTransform rect = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			float originLength;
			Vector2 currSize;
			Vector2 currScale;
			if (_layout == Layout.Vertical)
			{
				originLength = rect.get_sizeDelta().y;
				currSize = new Vector2(rect.get_sizeDelta().x, 0f);
				currScale = Vector2.op_Implicit(new Vector3(1f, 0f, 1f));
			}
			else
			{
				originLength = rect.get_sizeDelta().x;
				currSize = new Vector2(0f, rect.get_sizeDelta().y);
				currScale = Vector2.op_Implicit(new Vector3(0f, 1f, 1f));
			}
			float timer = 0f;
			rect.set_sizeDelta(currSize);
			((Transform)rect).set_localScale(new Vector3(currSize.x, currSize.y, 1f));
			while (timer < AnimationTime)
			{
				timer += Time.get_deltaTime();
				if (_layout == Layout.Vertical)
				{
					currScale.y = timer / AnimationTime;
					currSize.y = originLength * currScale.y;
				}
				else
				{
					currScale.x = timer / AnimationTime;
					currSize.x = originLength * currScale.x;
				}
				((Transform)rect).set_localScale(new Vector3(currScale.x, currScale.y, 1f));
				rect.set_sizeDelta(currSize);
				yield return null;
			}
			if (_layout == Layout.Vertical)
			{
				currScale.y = 1f;
				currSize.y = originLength;
			}
			else
			{
				currScale.x = 1f;
				currSize.x = originLength;
			}
			((Transform)rect).set_localScale(new Vector3(currScale.x, currScale.y, 1f));
			rect.set_sizeDelta(currSize);
		}

		private IEnumerator IERemoveItemAnim(GameObject item)
		{
			Transform transform = item.get_transform();
			RectTransform rect = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			float originLength;
			Vector2 currSize;
			Vector2 currScale;
			if (_layout == Layout.Vertical)
			{
				originLength = rect.get_sizeDelta().y;
				currSize = rect.get_sizeDelta();
				currScale = Vector2.op_Implicit(((Transform)rect).get_localScale());
			}
			else
			{
				originLength = rect.get_sizeDelta().x;
				currSize = rect.get_sizeDelta();
				currScale = Vector2.op_Implicit(((Transform)rect).get_localScale());
			}
			float timer = 0f;
			while (timer < AnimationTime)
			{
				timer += Time.get_deltaTime();
				if (_layout == Layout.Vertical)
				{
					currScale.y = 1f - timer / AnimationTime;
					currSize.y = originLength * currScale.y;
				}
				else
				{
					currScale.x = 1f - timer / AnimationTime;
					currSize.x = originLength * currScale.x;
				}
				((Transform)rect).set_localScale(new Vector3(currScale.x, currScale.y, 1f));
				rect.set_sizeDelta(currSize);
				yield return null;
			}
			((Transform)rect).SetParent((Transform)null);
			RemoveMethod(item);
		}
	}
}
